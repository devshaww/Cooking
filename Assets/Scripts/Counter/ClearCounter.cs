using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectOwner
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			if (player.HasKitchenObject()) {
				player.GetKitchenObject().SetOwner(this);
			}
		} else {
			// Pick up
			if (!player.HasKitchenObject()) {
				GetKitchenObject().SetOwner(player);
			} else {
				// Player holding a plate
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
					}
				} else { // Counter holding a plate, player adds ingredient to plate
					if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject1)) {
						if (plateKitchenObject1.TryAddIngredient(player.GetKitchenObject().KitchenObjectSO)) {
							KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
						}
					}
				}
			}
		}
	}
}
