/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Parent of the kitchen object(counters on which incridents are there) 
 */

using UnityEngine;

public interface IKitchenObjectParent {
    public Transform GetKitchenObjectFollowTrasform();
    public void SetKitchenObject(KitchenObject kitchenObject);
    public KitchenObject GetKitchenObject();
    public void ClearKitchenObject();
    public bool HasKitchenObject();
}
