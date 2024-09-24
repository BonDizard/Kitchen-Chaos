using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

    [SerializeField] private Transform topPointOfCounter;
    private KitchenObject kitchenObject;

    //virtual means the clid classes can implement this same method we can also use abstract also
    //protected key word means its like private bet the child class also can use it
    public virtual void Interact(PlayerScript player) {
        Debug.LogError("BaseCounter Interact");
    }
    public virtual void InteractAlternate(PlayerScript player) {
        Debug.LogError("BaseCounter InteractAlternate");

    }
    public Transform GetKitchenObjectFollowTrasform() {
        return topPointOfCounter;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }
    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }
    public void ClearKitchenObject() {
        kitchenObject = null;
    }
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

}
