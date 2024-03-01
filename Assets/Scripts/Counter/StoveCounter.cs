using System;
using System.Collections;
using System.Collections.Generic;
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
	private float fryingProgress;
	private State state = State.Idle;

    private void Update()
	{
		switch (state) {
		case State.Idle:
			break;
		case State.Frying:
			fryingProgress += Time.deltaTime;
			if (fryingProgress >= fryingRecipeSO.fryingTime) {
				state = State.Fried;
				OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state });
				fryingProgress = 0f;
				GetKitchenObject().DestroySelf();
				KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
			}
			OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = fryingProgress / fryingRecipeSO.fryingTime });
			break;
		case State.Fried:
			fryingProgress += Time.deltaTime;
			TryGetRecipeWithInput(fryingRecipeSO.output, out FryingRecipeSO recipeSO);
			if (fryingProgress >= recipeSO.fryingTime) {
				state = State.Burned;
				fryingProgress = 0f;
				OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state });
				GetKitchenObject().DestroySelf();
				KitchenObject.SpawnKitchenObject(recipeSO.output, this);
			}
			OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = fryingProgress / recipeSO.fryingTime });
			break;
		case State.Burned:
			break;
		}
	}

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			if (player.HasKitchenObject() && TryGetRecipeWithInput(player.GetKitchenObject().KitchenObjectSO, out FryingRecipeSO recipeSO))
			{
				fryingRecipeSO = recipeSO;
				player.GetKitchenObject().SetOwner(this);
				state = State.Frying;
				fryingProgress = 0f;
				OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = fryingProgress / recipeSO.fryingTime });
				OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state });
			}
		}
		else
		{
			// Pick up
			if (!player.HasKitchenObject() && state != State.Frying)
			{
				GetKitchenObject().SetOwner(player);
				state = State.Idle;
				OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = 0f });
				OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state });
			} else {
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						GetKitchenObject().DestroySelf();
						state = State.Idle;
						OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = 0f });
						OnStateChange?.Invoke(this, new OnStateChangeEventArgs { state = state });
					}	
				}
			}
		}
	}

	//public override void InteractAlternate(Player player)
	//{
	//	if (HasKitchenObject() && TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out FryingRecipeSO recipeSO))
	//	{
	//		fryingProgress++;
	//		OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = fryingProgress / recipeSO.fryingTime });
	//		KitchenObject kitchenObject = GetKitchenObject();
	//		if (fryingProgress >= recipeSO.fryingTime)
	//		{
	//			kitchenObject.DestroySelf();
	//			KitchenObject.SpawnKitchenObject(recipeSO.output, this);
	//		}
	//	}
	//}

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
}
