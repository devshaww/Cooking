using System;
using System.Collections;
using System.Collections.Generic;
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
		spawnTimer += Time.deltaTime;
		if (spawnTimer >= spawnTimeDuration) {
			spawnTimer = 0f;
			if (stackSize < maxStackSize) {
				stackSize += 1;
				OnPlateSpawn?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject()) {
			if (stackSize > 0) {
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
				stackSize -= 1;
				OnPlateTaken?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
