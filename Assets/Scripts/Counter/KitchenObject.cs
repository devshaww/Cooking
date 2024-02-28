using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
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

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectOwner parent) {
		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.visualPrefab);
		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
		kitchenObject.SetOwner(parent);
		return kitchenObject;
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
