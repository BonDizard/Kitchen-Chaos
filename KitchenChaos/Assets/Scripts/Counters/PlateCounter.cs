/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Plate counter logic
 */
using System;
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
        spawnPlateTime += Time.deltaTime;
        if (spawnPlateTime > spawnPlateTimeMax) {
            spawnPlateTime = 0f;

            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            //Player empty handed
            if (platesSpawnedAmount > 0) {
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObect(plateKitchenObjectSO, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
