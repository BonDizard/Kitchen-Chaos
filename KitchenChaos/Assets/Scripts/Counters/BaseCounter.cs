/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Base class for all counters
 */
using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {
    public static event EventHandler OnAnyObjectPlacedHere;
    public static void ResetStaticData() {
        OnAnyObjectPlacedHere = null;
    }
    [SerializeField] private Transform topPointOfCounter;
    private KitchenObject kitchenObject;

    //virtual means the child classes can implement this same method we can also use abstract also
    //protected key word means its like private but the child class also can use it
    public virtual void Interact(Player player) {
        Debug.LogError("BaseCounter Interact");
    }
    public virtual void InteractAlternate(Player player) {
        Debug.LogError("BaseCounter InteractAlternate");
    }
    public Transform GetKitchenObjectFollowTrasform() {
        return topPointOfCounter;
    }
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
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
