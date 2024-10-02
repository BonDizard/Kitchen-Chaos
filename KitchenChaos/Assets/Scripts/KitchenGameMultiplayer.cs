/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Kitchen objects network spawning (spawning ingredients)
 */
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {

    public const int MAX_PLAYERS_ALLOWED = 4;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiPlayer";
    public static KitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;
    [SerializeField] private List<Color> playerColors;


    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake() {
        Debug.Log("[DEBUG_KGM] Awake called - Initializing KitchenGameMultiplayer");
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        Debug.Log("[DEBUG_KGM] NetworkList initialized with PlayerData");
    }
    public string GetPlayerName() {
        return playerName;
    }
    public void SetPlayerName(string playerName) {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }
    private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        Debug.Log("[DEBUG_KGM] playerDataNetworkList_OnListChanged triggered: " + changeEvent + ", Element: " + changeEvent.Value);
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        Debug.Log("[DEBUG_KGM] StartHost called");
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Client disconnected: " + clientId);
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                Debug.Log("[DEBUG_KGM] Removing player data for clientId: " + clientId);
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Client connected: " + clientId);
        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
        SetPlayerNameServerRpc(GetPlayerName());
        Debug.Log("[DEBUG_KGM] Added player data for clientId: " + clientId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
        Debug.Log("[DEBUG_KGM] Connection approval request from client: " + connectionApprovalRequest.ClientNetworkId);

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS_ALLOWED) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            Debug.Log("[DEBUG_KGM] Connection denied: Game is full");
            return;
        }
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            Debug.Log("[DEBUG_KGM] Connection denied: Game has already started");
            return;
        }
        connectionApprovalResponse.Approved = true;
        Debug.Log("[DEBUG_KGM] Connection approved for client: " + connectionApprovalRequest.ClientNetworkId);
    }

    public void StartClient() {
        Debug.Log("[DEBUG_KGM] StartClient called");
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }
    public void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
        SetPlayerNameServerRpc(GetPlayerName());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = GetPlayerDataFromPlayerIndex(playerDataIndex);
        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerData;
    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Client failed to join or disconnected: " + clientId);
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnKitchenObect(KitchenObjectsSO kitchenObjectsSO, IKitchenObjectParent kitchenObjectParent) {
        Debug.Log("[DEBUG_KGM] Spawning kitchen object: " + kitchenObjectsSO.name);
        SpawnKitchenObectServerRpc(GetKitchenObjectSOsIndexFromList(kitchenObjectsSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnKitchenObectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        Debug.Log("[DEBUG_KGM] ServerRpc SpawnKitchenObjectServerRpc called for object index: " + kitchenObjectSOIndex);

        Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSOFromIndex(kitchenObjectSOIndex).prefab);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectToParent(kitchenObjectParent);

        Debug.Log("[DEBUG_KGM] Kitchen object spawned and set to parent");
    }

    public int GetKitchenObjectSOsIndexFromList(KitchenObjectsSO kitchenObjectsSO) {
        int index = kitchenObjectsListSO.kitchenObjectsSOsList.IndexOf(kitchenObjectsSO);
        Debug.Log("[DEBUG_KGM] GetKitchenObjectSOsIndexFromList: " + kitchenObjectsSO.name + " is at index " + index);
        return index;
    }

    public KitchenObjectsSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
        KitchenObjectsSO kitchenObjectSO = kitchenObjectsListSO.kitchenObjectsSOsList[kitchenObjectSOIndex];
        Debug.Log("[DEBUG_KGM] GetKitchenObjectSOFromIndex: Index " + kitchenObjectSOIndex + " corresponds to " + kitchenObjectSO.name);
        return kitchenObjectSO;
    }

    public void DestroyKitchenObect(KitchenObject kitchenObject) {
        Debug.Log("[DEBUG_KGM] Destroying kitchen object: " + kitchenObject.name);
        DestroyKitchenObectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyKitchenObectServerRpc(NetworkObjectReference kitchenNetworkObjectrefrence) {
        Debug.Log("[DEBUG_KGM] ServerRpc DestroyKitchenObjectServerRpc called");

        kitchenNetworkObjectrefrence.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObectClientRpc(kitchenNetworkObjectrefrence);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObectClientRpc(NetworkObjectReference kitchenNetworkObjectrefrence) {
        Debug.Log("[DEBUG_KGM] ClientRpc ClearKitchenObjectClientRpc called");

        kitchenNetworkObjectrefrence.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex) {
        Debug.Log("[DEBUG_KGM] Checking if player index " + playerIndex + " is connected");
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Getting player data index for clientId: " + clientId);
        Debug.Log("[DEBUG_KGM] " + playerDataNetworkList[0].clientId);

        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                Debug.Log("[DEBUG_KGM] Found player data at index " + i + " for clientId: " + clientId);
                return i;
            }
        }
        Debug.Log("[DEBUG_KGM] Player data not found for clientId: " + clientId);
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Getting player data for clientId: " + clientId);
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.clientId == clientId) {
                Debug.Log("[DEBUG_KGM] Found player data for clientId: " + clientId);
                return playerData;
            }
        }
        Debug.Log("[DEBUG_KGM] No player data found for clientId: " + clientId);
        return default;
    }

    public PlayerData GetPlayerData() {
        Debug.Log("[DEBUG_KGM] Getting local player data");
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
        Debug.Log("[DEBUG_KGM] Getting player data for playerIndex: " + playerIndex);
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int playerIndex) {
        Debug.Log("[DEBUG_KGM] Getting player color for playerIndex: " + playerIndex);
        return playerColors[playerIndex];
    }

    public void ChangePlayerColor(int colorId) {
        Debug.Log("[DEBUG_KGM] Changing player color to colorId: " + colorId);
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
        Debug.Log("[DEBUG_KGM] ServerRpc ChangePlayerColorServerRpc called for colorId: " + colorId);

        if (!IsColorAvailable(colorId)) {
            Debug.Log("[DEBUG_KGM] Color " + colorId + " is already in use");
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
        Debug.Log("[DEBUG_KGM] Player color changed to colorId: " + colorId);
    }

    private bool IsColorAvailable(int colorId) {
        Debug.Log("[DEBUG_KGM] Checking if colorId " + colorId + " is available");
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.colorId == colorId) {
                Debug.Log("[DEBUG_KGM] ColorId " + colorId + " is already in use");
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId() {
        Debug.Log("[DEBUG_KGM] Getting first unused colorId");
        for (int i = 0; i < playerColors.Count; i++) {
            if (IsColorAvailable(i)) {
                Debug.Log("[DEBUG_KGM] ColorId " + i + " is unused and available");
                return i;
            }
        }
        Debug.Log("[DEBUG_KGM] No unused colorId available");
        return -1;
    }

    public void KickPlayer(ulong clientId) {
        Debug.Log("[DEBUG_KGM] Kicking player with clientId: " + clientId);
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
