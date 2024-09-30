/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Kitchen objects network spwaning (spawining ingridents)
 */
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {
    public event EventHandler OnTryingToConnect;
    public event EventHandler OnFailedToConnect;
    private const int MAX_PLAYERS_ALLOWED = 4;
    public static KitchenGameMultiplayer Instance { get; private set; }
    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS_ALLOWED) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        if (SceneManager.GetActiveScene().name == Loader.Scene.GameScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already started";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
    public void StartClient() {
        OnTryingToConnect?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        OnFailedToConnect?.Invoke(this, EventArgs.Empty);
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
