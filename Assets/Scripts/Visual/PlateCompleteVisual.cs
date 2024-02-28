using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
	[Serializable]
	public struct KitchenObjectSO_GameObject {
		public KitchenObjectSO kitchenObjectSO;
		public GameObject gameObject;
	}

	[SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;

    [SerializeField] private PlateKitchenObject plateKitchenObject;

    void Start()
    {
		plateKitchenObject.OnIngredientAdd += PlateKitchenObject_OnIngredientAdd;
    }

	private void PlateKitchenObject_OnIngredientAdd(object sender, PlateKitchenObject.OnIngredientAddEventArgs e)
	{
		foreach (KitchenObjectSO_GameObject kg in kitchenObjectSOGameObjectList) {
			if (kg.kitchenObjectSO == e.kitchenObjectSO) {
				kg.gameObject.SetActive(true);
			}
		}
	}
}
