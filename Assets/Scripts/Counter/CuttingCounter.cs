using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IProgressible
{
	[SerializeField] private CuttingRecipeSO[] cuttingRecipes;

	public static event EventHandler OnAnyCut;
	public event EventHandler OnCut;
	public event EventHandler<IProgressible.OnProgressChangeEventArgs> OnProgressUpdate;

	private int cuttingProgress;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject()) {
			if (player.HasKitchenObject()) {
				player.GetKitchenObject().SetOwner(this);
				if (TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out CuttingRecipeSO recipeSO)) {
					cuttingProgress = 0;
					OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = cuttingProgress / recipeSO.cuttingTime });
				}
			}
		} else { 
			// Pick up
			if (!player.HasKitchenObject())
			{
				GetKitchenObject().SetOwner(player);
			} else {
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						GetKitchenObject().DestroySelf();
					}
				}
			}
		}
	}

	public override void InteractAlternate(Player player)
	{
		if (HasKitchenObject() && TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out CuttingRecipeSO recipeSO)) {
			cuttingProgress++;
			OnCut?.Invoke(this, EventArgs.Empty);
			OnAnyCut?.Invoke(this, EventArgs.Empty);
			OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = cuttingProgress / recipeSO.cuttingTime });
			KitchenObject kitchenObject = GetKitchenObject();
			if (cuttingProgress >= recipeSO.cuttingTime) {
				kitchenObject.DestroySelf();
				KitchenObject.SpawnKitchenObject(recipeSO.output, this);
			}
		}
	}

	public bool TryGetRecipeWithInput(KitchenObjectSO input, out CuttingRecipeSO recipeSO)
	{
		foreach (CuttingRecipeSO so in cuttingRecipes) {
			if (so.input == input) {
				recipeSO = so;
				return true;
			}
		}
		recipeSO = null;
		return false;
	}

	new public static void ResetStaticData() {
		OnAnyCut = null;
	}
}
