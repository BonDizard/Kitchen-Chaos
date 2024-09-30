/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Kitchen objects network spwaning (spawining ingridents)
 */
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {
    public event EventHandler OnTryingToConnect;
    public event EventHandler OnFailedToConnect;
    public event EventHandler OnPlayerListChanged;
    private const int MAX_PLAYERS_ALLOWED = 4;
    public static KitchenGameMultiplayer Instance { get; private set; }
    private NetworkList<PlayerData> playerDataNetworkList;
    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;
    [SerializeField] private List<Color> playerColors;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;
    }

    private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                //this player disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        playerDataNetworkList.Add(
            new PlayerData {
                clientId = clientId,
                colorId = GetFirstUnusedColorId(),
            });
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

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < playerDataNetworkList.Count;
    }
    public PlayerData GetPlayerDataFromIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }
    public Color GetPlayerColor(int playerIndex) {
        return playerColors[playerIndex];
    }
    public PlayerData GetPlayerData() {
        return GetPlayerDataFromClientId(
        NetworkManager.Singleton.LocalClientId);
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.clientId == clientId) {
                return playerData;
            }
        }
        return default;
    }
    public void ChangePlayerColor(int colorId) {

    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorId)) {
            //color already used
            return;
        }
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    private bool IsColorAvailable(int colorId) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.colorId == colorId) {
                return false;
            }
        }
        return true;
    }
    private int GetFirstUnusedColorId() {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (IsColorAvailable(playerDataNetworkList[i].colorId)) {
                return i;
            }
        }
        return -1;
    }
    public void KickPlayer(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
