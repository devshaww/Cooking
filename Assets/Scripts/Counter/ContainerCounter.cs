using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	public event EventHandler OnPlayerGrabObject;

	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
		// Play openclose animation
		OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
	}
}
