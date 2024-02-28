using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

	private void Awake()
	{
        iconTemplate.gameObject.SetActive(false);
	}

	public void SetRecipeSO(RecipeSO recipeSO) {
        recipeNameText.text = recipeSO.name;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject); 
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOs) {
            Transform iconTemplateTransform = Instantiate(iconTemplate, iconContainer);
            iconTemplateTransform.gameObject.SetActive(true);
            iconTemplateTransform.GetComponent<DMIconTemplateSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}
