using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
	[SerializeField] private Transform iconTemplate;

	private void Awake()
	{
		iconTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		plateKitchenObject.OnIngredientAdd += PlateKitchenObject_OnIngredientAdd;
	}

	private void PlateKitchenObject_OnIngredientAdd(object sender, PlateKitchenObject.OnIngredientAddEventArgs e)
	{
		UpdateVisual();
	}

	public void UpdateVisual() {
		foreach (Transform child in transform) {
			if (child != iconTemplate) {
				Destroy(child.gameObject);
			}
		}
		foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
			Transform iconTemplateTransform = Instantiate(iconTemplate, transform);
			iconTemplateTransform.gameObject.SetActive(true);
			iconTemplateTransform.GetComponent<OnPlateSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
		}
	}
}
