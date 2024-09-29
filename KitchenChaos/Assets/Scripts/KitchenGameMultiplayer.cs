/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Kitchen objects network spwaning (spawining ingridents)
 */
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour {
    public static KitchenGameMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;
    private void Awake() {
        Instance = this;
    }
    public void SpawnKitchenObect(KitchenObjectsSO kitchenObjectsSO, IKitchenObjectParent kitchenObjectParent) {
        SpawnKitchenObectServerRpc(GetKitchenObjectSOsIndexFromList(kitchenObjectsSO), kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnKitchenObectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        // Instantiate the prefab from the KitchenObjectsSO.
        Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSOFromIndex(kitchenObjectSOIndex).prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        // Get the KitchenObject component from the instantiated prefab.
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        // Set the new kitchen object to its parent.
        kitchenObject.SetKitchenObjectToParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOsIndexFromList(KitchenObjectsSO kitchenObjectsSO) {
        return kitchenObjectsListSO.kitchenObjectsSOsList.IndexOf(kitchenObjectsSO);
    }
    public KitchenObjectsSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
        return kitchenObjectsListSO.kitchenObjectsSOsList[kitchenObjectSOIndex];
    }
    public void DestroyKitchenObect(KitchenObject kitchenObject) {
        DestroyKitchenObectServerRpc(kitchenObject.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyKitchenObectServerRpc(NetworkObjectReference kitchenNetworkObjectrefrence) {
        kitchenNetworkObjectrefrence.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObectClientRpc(kitchenNetworkObjectrefrence);
        kitchenObject.DestroySelf();
    }
    [ClientRpc]
    private void ClearKitchenObectClientRpc(NetworkObjectReference kitchenNetworkObjectrefrence) {
        kitchenNetworkObjectrefrence.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }
}
