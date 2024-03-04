using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	public event EventHandler OnPlayerGrabObject;

	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

		// Play openclose animation
		InteractLogicServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc() {
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc() {		
		OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
	}
}
