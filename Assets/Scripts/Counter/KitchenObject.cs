using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectOwner owner;

    public KitchenObjectSO KitchenObjectSO { get => kitchenObjectSO; }

	public IKitchenObjectOwner GetOwner() {
        return owner;    
    }

    public void SetOwner(IKitchenObjectOwner owner) {
		this.owner?.ClearKitchenObject();
        if (owner.HasKitchenObject()) {
            Debug.LogError("Try to own more than one objects");
        }
		this.owner = owner;
		owner.SetKitchenObject(this);
		transform.parent = owner.GetSpawnPoint();
		transform.localPosition = Vector3.zero;
    }

    public void DestroySelf() {
        owner.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner parent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, parent);
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

}
