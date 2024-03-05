using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
	public static event EventHandler OnAnyObjectTrashed;

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject()) {
			KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
			InteractLogicServerRpc();
		}
	}

	new public static void ResetStaticData() {
		OnAnyObjectTrashed = null;
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc() {
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc() {
		OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
	}
}
