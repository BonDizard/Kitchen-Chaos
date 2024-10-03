/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Plate counter logic
 */
using System;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter {
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectsSO plateKitchenObjectSO;
    private float spawnPlateTime;
    private float spawnPlateTimeMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    private void Update() {
        //spawn plate only on server
        if (!IsServer) {
            return;
        }
        spawnPlateTime += Time.deltaTime;
        if (spawnPlateTime > spawnPlateTimeMax) {
            spawnPlateTime = 0f;

            if (KitchenGameManger.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                PlateSpawnServerRpc();
            }
        }
    }
    //technically only server calls this methos from update but still lets keep the format we are using
    [ServerRpc]
    private void PlateSpawnServerRpc() {
        PlateSpawnClientRpc();
    }
    [ClientRpc]
    private void PlateSpawnClientRpc() {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            //Player empty handed
            if (platesSpawnedAmount > 0) {
                KitchenObject.SpawnKitchenObect(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
