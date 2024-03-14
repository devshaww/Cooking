using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    [SerializeField] private RecipeListSO recipelistSO;
	public static DeliveryManager Instance { get; private set; }

	// Events for updating order list visual
	public event EventHandler OnRecipeSpawn;
	public event EventHandler OnRecipeComplete;

	// SFX and DeliveryCounter visual Events
	public event EventHandler OnRecipeSucceed;
	public event EventHandler OnRecipeFail;

    private List<RecipeSO> waitingRecipsSOs;

	private float spawnRecipeTimer;
	private float spawnRecipeTimerMax = 4f;
	private int maxSpawn = 4;
	private int deliveredCount;

	private void Awake()
	{
		Instance = this;
		waitingRecipsSOs = new List<RecipeSO>();
	}

	private void Update()
	{
		if (!IsServer) {
			return;
		}
		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0) {
			//SpawnRecipe();
			spawnRecipeTimer = spawnRecipeTimerMax;
			if (GameManager.Instance.IsGamePlaying() && waitingRecipsSOs.Count < maxSpawn) {
				int randomIndex = UnityEngine.Random.Range(0, recipelistSO.recipeSOList.Count);
				SpawnNewWatingRecipeClientRpc(randomIndex);
			}
		}
	}

	[ClientRpc]
	private void SpawnNewWatingRecipeClientRpc(int index) {
		RecipeSO recipeSO = recipelistSO.recipeSOList[index];
		waitingRecipsSOs.Add(recipeSO);
		OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
	}

	//private void SpawnRecipe() {
	//	if (waitingRecipsSOs.Count < maxSpawn) {
	//		RecipeSO recipeSO = recipelistSO.recipeSOList[UnityEngine.Random.Range(0, recipelistSO.recipeSOList.Count)];
	//		waitingRecipsSOs.Add(recipeSO);
	//		OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
	//	}
	//}

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
					DeliverSuccessServerRpc(i);
					return true;
				}
			}
		}
		DeliverFailedServerRpc();
		return false;
	}

	[ServerRpc(RequireOwnership = false)]
	private void DeliverSuccessServerRpc(int indexToRemove) {
		DeliverSuccessClientRpc(indexToRemove);
	}

	[ClientRpc]
	private void DeliverSuccessClientRpc(int indexToRemove) {
		deliveredCount++;
		waitingRecipsSOs.RemoveAt(indexToRemove);
		OnRecipeComplete?.Invoke(this, EventArgs.Empty);
		OnRecipeSucceed?.Invoke(this, EventArgs.Empty);
	}

	[ServerRpc(RequireOwnership = false)]
	private void DeliverFailedServerRpc() {
		DeliverFailedClientRpc();
	}

	[ClientRpc]
	private void DeliverFailedClientRpc() {
		OnRecipeFail?.Invoke(this, EventArgs.Empty);
	}

	public List<RecipeSO> GetWaitingRecipeSOList() {
		return waitingRecipsSOs;
	}

	public int GetDeliveredCount() {
		return deliveredCount;
	}
}
