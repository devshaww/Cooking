using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectOwner
{
    public void SetKitchenObject(KitchenObject ko);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();

    public bool HasKitchenObject();

    public Transform GetSpawnPoint();

    public NetworkObject GetNetworkObject();
}
