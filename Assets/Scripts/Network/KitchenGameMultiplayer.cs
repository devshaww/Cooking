using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    public static KitchenGameMultiplayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner owner) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), owner.GetNetworkObject());
	}

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int index, NetworkObjectReference ownerRef) {
        KitchenObjectSO kitchenObjectSO = kitchenObjectListSO.kitchenObjectSOList[index];
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.visualPrefab);
        NetworkObject networkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        // code for parenting only runs on server side, so it needs another synchronization.
        // client tells server the kitchen object's owner and server broadcast to other clients to sync.
		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        ownerRef.TryGet(out NetworkObject ownerNetworkObject);
        IKitchenObjectOwner owner = ownerNetworkObject.GetComponent<IKitchenObjectOwner>();
        kitchenObject.SetOwner(owner);
    }
}
