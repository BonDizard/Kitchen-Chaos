/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character ready logic
 */
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSetReady : NetworkBehaviour {
    public static CharacterSetReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake() {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsAreReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerReadyDictionary[clientId] || playerReadyDictionary.ContainsKey(clientId)) {
                //some player not ready
                allClientsAreReady = false;
                break;
            }
        }
        if (allClientsAreReady) {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
}

