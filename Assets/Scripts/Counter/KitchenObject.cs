using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectOwner owner;
    private FollowTransform followTransform;

    public KitchenObjectSO KitchenObjectSO { get => kitchenObjectSO; }

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public IKitchenObjectOwner GetOwner() {
        return owner;    
    }

    public void SetOwner(IKitchenObjectOwner owner) {
        SetOwnerServerRpc(owner.GetNetworkObject());
    }

    public void DestroySelf() {        
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnOwner() {
        owner.ClearKitchenObject();
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner parent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, parent);
	}

    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
	}

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetOwnerServerRpc(NetworkObjectReference ownerRef) {
        SetOwnerClientRpc(ownerRef);       
    }

    [ClientRpc]
    private void SetOwnerClientRpc(NetworkObjectReference ownerRef) {
        ownerRef.TryGet(out NetworkObject ownerNetworkObject);
        IKitchenObjectOwner owner = ownerNetworkObject.GetComponent<IKitchenObjectOwner>();

        this.owner?.ClearKitchenObject();
        if (owner.HasKitchenObject()) {
            Debug.LogError("Try to own more than one objects");
        }
		this.owner = owner;
		owner.SetKitchenObject(this);
        //transform.parent = owner.GetSpawnPoint();
        //transform.localPosition = Vector3.zero;
        // 设置owner的spawnpoint的transform，并在LateUpdate里根据spawnpoint的transform不断更新
        followTransform.SetTargetTransform(owner.GetSpawnPoint());
    }

}
