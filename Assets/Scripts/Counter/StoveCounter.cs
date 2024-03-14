using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IProgressible
{
	[SerializeField] private FryingRecipeSO[] fryingRecipes;

	public event EventHandler<OnStateChangeEventArgs> OnStateChange;
	public class OnStateChangeEventArgs : EventArgs { public State state; }
	public event EventHandler<IProgressible.OnProgressChangeEventArgs> OnProgressUpdate;

	public enum State {
		Idle,
		Frying,
		Fried,
		Burned
	}

	// current recipe with input and output and fryingTime
	private FryingRecipeSO fryingRecipeSO;
	private NetworkVariable<float> fryingProgress = new(0f);
	private NetworkVariable<State> state = new(State.Idle);

	public override void OnNetworkSpawn() {
		fryingProgress.OnValueChanged += FryingProgress_OnValueChanged;
		state.OnValueChanged += State_OnValueChanged;
	}

	private void FryingProgress_OnValueChanged(float oldValue, float newValue) {
		float fryingTime = fryingRecipeSO != null ? fryingRecipeSO.fryingTime : 1f;
		OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = fryingProgress.Value / fryingTime });
	}

	private void State_OnValueChanged(State oldValue, State newValue) {
		OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state.Value });
		if (state.Value == State.Burned || state.Value == State.Idle) {
			OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = 0f });
		}		
	}

    private void Update()
	{
		if (!IsServer) { return; }
		switch (state.Value) {
		case State.Idle:
			break;
		case State.Frying:
			fryingProgress.Value += Time.deltaTime;
			if (fryingProgress.Value >= fryingRecipeSO.fryingTime) {
				state.Value = State.Fried;
				fryingProgress.Value = 0f;
				KitchenObject.DestroyKitchenObject(GetKitchenObject());
				KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
			}
			break;
		case State.Fried:
			fryingProgress.Value += Time.deltaTime;
			TryGetRecipeWithInput(fryingRecipeSO.output, out FryingRecipeSO recipeSO);
			if (fryingProgress.Value >= recipeSO.fryingTime) {
				state.Value = State.Burned;
				fryingProgress.Value = 0f;
				KitchenObject.DestroyKitchenObject(GetKitchenObject());
				KitchenObject.SpawnKitchenObject(recipeSO.output, this);
			}
			break;
		case State.Burned:
			break;
		}
	}

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			if (player.HasKitchenObject()) {
				KitchenObject ko = player.GetKitchenObject();
				ko.SetOwner(this);
				SetFryingRecipeSOServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(ko.KitchenObjectSO));			
			}
		}
		else
		{
			// Pick up
			if (!player.HasKitchenObject() && state.Value != State.Frying)
			{
				GetKitchenObject().SetOwner(player);
				SetStateIdleServerRpc();
			} else {
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
						SetStateIdleServerRpc();
					}	
				}
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetStateIdleServerRpc() {
		state.Value = State.Idle;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetFryingRecipeSOServerRpc(int index) {
		fryingProgress.Value = 0f;
		state.Value = State.Frying;
		SetFryingRecipeSOClientRpc(index);
	}

	[ClientRpc]
	private void SetFryingRecipeSOClientRpc(int index) {
		fryingRecipeSO = GetRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKitchenObjectSOAtIndex(index));		
	}

	public FryingRecipeSO GetRecipeSOWithInput(KitchenObjectSO input) {
		foreach (FryingRecipeSO so in fryingRecipes)
		{
			if (so.input == input)
			{
				return so;
			}
		}
		Debug.LogError("FryingRecipeSO with input not found");
		return null;
	}

	public bool TryGetRecipeWithInput(KitchenObjectSO input, out FryingRecipeSO recipeSO)
	{
		foreach (FryingRecipeSO so in fryingRecipes)
		{
			if (so.input == input)
			{
				recipeSO = so;
				return true;
			}
		}
		recipeSO = null;
		return false;
	}

	public bool IsFried() {
		return state.Value == State.Fried;
	}
}
