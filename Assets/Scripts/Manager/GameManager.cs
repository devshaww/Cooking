using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public event EventHandler OnStateChange;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new(State.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new(3f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    private float gamePlayingTimerMax = 60f;
    private bool isGamePaused = false;
    private bool isLocalPlayerReady;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        InputHandler.Instance.OnPauseAction += InputHandler_OnPauseAction;
        InputHandler.Instance.OnInteractAction += InputHandler_OnInteractAction;
        //state = State.CountdownToStart;
        //OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State oldState, State newState) {
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    private void InputHandler_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void InputHandler_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart) {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);  // Show WaitingForPlayersUI
            SetPlayerReadyServerRpc();
            // 这里关于先发rpc再显示WaitingForPlayersUI的顺序的注意点是
            // 如果是先服务器ready再client OnLocalPlayerReadyChanged的调用会在statechanged之前 所以WaitingForPlayersUI的隐藏正常工作
            // 如果先client后服务器 服务器的OnLocalPlayerReadyChanged的调用会在statechanged之后 所以server端的WaitingForPlayersUI又会显示出来
            // 大概是因为如果是ServerRpc 服务器需要自己执行 所以是同步 而client发了rpc后可以直接运行后面的代码            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong cliendId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(cliendId) || !playerReadyDictionary[cliendId]) {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    private void Update()
    {
        if (!IsServer) { return; }
        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f) {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStart() {
        return state.Value == State.CountdownToStart;
    }

    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }

    public bool IsLocalPlayerReady() {
        return isLocalPlayerReady;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer.Value;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - gamePlayingTimer.Value / gamePlayingTimerMax;
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        
    }
}
