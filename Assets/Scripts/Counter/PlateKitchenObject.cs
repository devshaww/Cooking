using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    // KitchenObjectSOs that can be put on a plate
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOs;
    private List<KitchenObjectSO> kitchenObjectSOs;

    public event EventHandler<OnIngredientAddEventArgs> OnIngredientAdd;
    public class OnIngredientAddEventArgs: EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

	protected override void Awake()
	{
        base.Awake();
        kitchenObjectSOs = new List<KitchenObjectSO>();
	}

	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOs.Contains(kitchenObjectSO)) {
            return false;
        }
        if (!kitchenObjectSOs.Contains(kitchenObjectSO)) {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
        return false;  
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int index) {
        AddIngredientClientRpc(index);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int index) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOAtIndex(index);
        kitchenObjectSOs.Add(kitchenObjectSO);
        OnIngredientAdd?.Invoke(this, new OnIngredientAddEventArgs { kitchenObjectSO = kitchenObjectSO });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOs;
    }
}
