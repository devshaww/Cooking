using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    public static GamePauseUI Instance;

    // cannot deactivate in Awake() because that way Start() won't get called(Start() called on activated object)
    private void Awake()
    {
        Instance = this;
        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show();
        });

        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    public void Show() {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }

    public void Hide() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
