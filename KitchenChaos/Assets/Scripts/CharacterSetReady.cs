/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character ready logic
 */
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSetReady : NetworkBehaviour {
    public static CharacterSetReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDictionary;
    public event EventHandler OnReadyChanged;

    private void Awake() {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
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
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId) {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId) {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}

