using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
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
            kitchenObjectSOs.Add(kitchenObjectSO);
            OnIngredientAdd?.Invoke(this, new OnIngredientAddEventArgs { kitchenObjectSO = kitchenObjectSO });
            return true;
        }
        return false;  
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOs;
    }
}
