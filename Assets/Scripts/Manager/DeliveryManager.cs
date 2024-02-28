using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private RecipeListSO recipelistSO;
	public static DeliveryManager Instance { get; private set; }
	public event EventHandler OnRecipeSpawn;
	public event EventHandler OnRecipeComplete;

	// SFX Events
	public event EventHandler OnRecipeSucceed;
	public event EventHandler OnRecipeFail;

    private List<RecipeSO> waitingRecipsSOs;

	private float spawnRecipeTimer = 4f;
	private float spawnRecipeTimerMax = 4f;
	private int maxSpawn = 4;

	private void Awake()
	{
		Instance = this;
		waitingRecipsSOs = new List<RecipeSO>();
	}

	private void Update()
	{
		spawnRecipeTimer += Time.deltaTime;
		if (spawnRecipeTimer >= spawnRecipeTimerMax) {
			SpawnRecipe();
			spawnRecipeTimer = 0f;
		}
	}

	private void SpawnRecipe() {
		if (waitingRecipsSOs.Count < maxSpawn) {
			RecipeSO recipeSO = recipelistSO.recipeSOList[UnityEngine.Random.Range(0, recipelistSO.recipeSOList.Count)];
			waitingRecipsSOs.Add(recipeSO);
			OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool DeliverRecipe(PlateKitchenObject plateKitchenObject) {
		for (int i = 0; i < waitingRecipsSOs.Count; i++) {
			RecipeSO waitingRecipeSO = waitingRecipsSOs[i];
			if (waitingRecipeSO.kitchenObjectSOs.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
				bool plateContentsMatchesRecipe = true;
				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOs) {
					bool ingredientFound = false;
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
						if (recipeKitchenObjectSO == plateKitchenObjectSO) {
							ingredientFound = true;
							break;
						}
					}
					if (!ingredientFound) {
						plateContentsMatchesRecipe = false;
						break;
					}
				}
				if (plateContentsMatchesRecipe) {
					Debug.Log("Delivered");
					waitingRecipsSOs.RemoveAt(i);
					OnRecipeComplete?.Invoke(this, EventArgs.Empty);
					OnRecipeSucceed?.Invoke(this, EventArgs.Empty);
					return true;
				}
			}
		}
		// No matches found
		Debug.Log("No matches found");
		OnRecipeFail?.Invoke(this, EventArgs.Empty);
		return false;
	}

	public List<RecipeSO> GetWaitingRecipeSOList() {
		return waitingRecipsSOs;
	}
}
