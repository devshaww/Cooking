using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
				KitchenObject ko = player.GetKitchenObject();
				ko.SetOwner(this);
				ProgressResetServerRpc();
			}
		} else { 
			// Pick up
			if (!player.HasKitchenObject())
			{
				GetKitchenObject().SetOwner(player);
			} else {
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
					}
				}
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void ProgressResetServerRpc() {
		ProgressResetClientRpc();
	}

	[ClientRpc]
	private void ProgressResetClientRpc() {
		cuttingProgress = 0;
		OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs {
			progressNormalized = 0f
		});
	}

	public override void InteractAlternate(Player player)
	{
		if (HasKitchenObject() && TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out CuttingRecipeSO recipeSO))
		{
			CutObjectServerRpc();
			TestCuttingProgressDoneServerRpc();
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

	[ServerRpc(RequireOwnership = false)]
	private void CutObjectServerRpc() {
		CutObjectClientRpc();
	}

	[ClientRpc]
	private void CutObjectClientRpc() {
		cuttingProgress++;
		OnCut?.Invoke(this, EventArgs.Empty);
		OnAnyCut?.Invoke(this, EventArgs.Empty);
		TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out CuttingRecipeSO recipeSO);
		OnProgressUpdate?.Invoke(this, new IProgressible.OnProgressChangeEventArgs { progressNormalized = cuttingProgress / recipeSO.cuttingTime });		
	}

	[ServerRpc(RequireOwnership = false)]
	private void TestCuttingProgressDoneServerRpc() {
		TryGetRecipeWithInput(GetKitchenObject().KitchenObjectSO, out CuttingRecipeSO recipeSO);
		if (cuttingProgress >= recipeSO.cuttingTime) {
			KitchenObject.DestroyKitchenObject(GetKitchenObject());
			KitchenObject.SpawnKitchenObject(recipeSO.output, this);
		}		
	}
}
