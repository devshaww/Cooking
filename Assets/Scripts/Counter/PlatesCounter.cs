using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO plateKitchenObjectSO;

	public event EventHandler OnPlateSpawn;
	public event EventHandler OnPlateTaken;

	private int maxStackSize = 5;
	private int stackSize;
	private float spawnTimeDuration = 2f;
	private float spawnTimer;

	private void Update()
	{
		if (!IsServer) { return; }
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnTimeDuration) {
			spawnTimer = 0f;
			if (stackSize < maxStackSize) {
				SpawnPlateServerRpc();
			}
		}
	}

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject()) {
			if (stackSize > 0) {
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
				TakePlateServerRpc();				
			}
		}
	}

	[ServerRpc]
	private void SpawnPlateServerRpc() {
		SpawnPlateClientRpc();
	}

	[ClientRpc]
	private void SpawnPlateClientRpc() {
		stackSize += 1;
		OnPlateSpawn?.Invoke(this, EventArgs.Empty);
	}

	[ServerRpc(RequireOwnership = false)]
	private void TakePlateServerRpc() {
		TakePlateClientRpc();
	}

	[ClientRpc]
	private void TakePlateClientRpc() {		
		stackSize -= 1;
		OnPlateTaken?.Invoke(this, EventArgs.Empty);
	}
}
