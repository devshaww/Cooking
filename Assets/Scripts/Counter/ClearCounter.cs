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
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().KitchenObjectSO)) {
						GetKitchenObject().DestroySelf();
					}
				} else {
					if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject1)) {
						if (plateKitchenObject1.TryAddIngredient(player.GetKitchenObject().KitchenObjectSO)) {
							player.GetKitchenObject().DestroySelf();
						}
					}
				}
			}
		}
	}
}
