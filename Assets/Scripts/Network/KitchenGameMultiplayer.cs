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

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner parent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), parent.GetNetworkObject());
	}

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int index, NetworkObjectReference parentReference) {
        KitchenObjectSO kitchenObjectSO = kitchenObjectListSO.kitchenObjectSOList[index];
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.visualPrefab);
        NetworkObject networkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        parentReference.TryGet(out NetworkObject parentNetworkObject);
        IKitchenObjectOwner parent = parentNetworkObject.GetComponent<IKitchenObjectOwner>();
        kitchenObject.SetOwner(parent);
    }
}
