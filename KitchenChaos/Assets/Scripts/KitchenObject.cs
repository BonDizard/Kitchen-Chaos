/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Code for handling kitchen object interactions, including spawning, assigning to parent objects, and destruction.
 */

using UnityEngine;

public class KitchenObject : MonoBehaviour {

    // A serialized field to assign a KitchenObjectsSO ScriptableObject in the Unity editor.
    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;
    private IKitchenObjectParent kitchenObjectParent;
    public KitchenObjectsSO GetKitchenObjectSO() {
        return kitchenObjectsSO;
    }

    // Sets the parent object for this kitchen object and clears the old parent, if any.
    public void SetKitchenObjectToParent(IKitchenObjectParent kitchenObjectParent) {
        // If there is already a parent, clear the current kitchen object reference in the parent.
        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        // Assign the new parent.
        this.kitchenObjectParent = kitchenObjectParent;

        // Check if the new parent already has a kitchen object assigned.
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.Log("IKitchenObjectParent already has an object.");
        }

        // Set this object as the kitchen object for the new parent.
        kitchenObjectParent.SetKitchenObject(this);

        // Set the transform of this kitchen object to follow the parent's transform.
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTrasform();
        transform.localPosition = Vector3.zero;  // Reset local position to zero to ensure proper placement.
    }

    // Getter method to retrieve the current parent of this kitchen object.
    public IKitchenObjectParent GetClearCounter() {
        return kitchenObjectParent;
    }

    // Destroys this kitchen object and clears the reference in the parent object.
    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject(); // Clear the reference in the parent.
        Destroy(gameObject); // Destroy the game object in the scene.
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {

        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else {
            plateKitchenObject = null;
            return false;
        }
    }
    // Static method to spawn a new kitchen object based on a provided KitchenObjectsSO and assign it to a parent.
    public static KitchenObject SpawnKitchenObect(KitchenObjectsSO kitchenObjectsSO, IKitchenObjectParent kitchenObjectParent) {
        // Instantiate the prefab from the KitchenObjectsSO.
        Transform kitchenObjectTransform = Instantiate(kitchenObjectsSO.prefab);

        // Get the KitchenObject component from the instantiated prefab.
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        // Set the new kitchen object to its parent.
        kitchenObject.SetKitchenObjectToParent(kitchenObjectParent);

        // Return the created kitchen object.
        return kitchenObject;
    }
}
