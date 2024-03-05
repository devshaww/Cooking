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

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOAtIndex(int index) {
        return kitchenObjectListSO.kitchenObjectSOList[index];
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner owner) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), owner.GetNetworkObject());
	}

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
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

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectRef) {
        kitchenObjectRef.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject ko = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnOwnerClientRpc(kitchenObjectRef);
        ko.DestroySelf();        
    }

    [ClientRpc]
    private void ClearKitchenObjectOnOwnerClientRpc(NetworkObjectReference kitchenObjectRef) {
        kitchenObjectRef.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject ko = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ko.ClearKitchenObjectOnOwner();
    }
}
