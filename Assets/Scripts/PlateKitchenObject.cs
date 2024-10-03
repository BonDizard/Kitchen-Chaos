/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: plate objects behaviour
 */

using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredeintAddedEventArgs> OnIngredeintAdded;
    public class OnIngredeintAddedEventArgs : EventArgs {
        public KitchenObjectsSO kitchenObjectSO;
    }
    List<KitchenObjectsSO> kitchenObjectsSOList;
    [SerializeField] private List<KitchenObjectsSO> validKitchenObjectSOList;
    protected override void Awake() {
        base.Awake();
        kitchenObjectsSOList = new List<KitchenObjectsSO>();
    }

    public bool TryAddIngredients(KitchenObjectsSO kitchenObjectsSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectsSO)) {
            //not a valid ingredient
            return false;
        }
        if (kitchenObjectsSOList.Contains(kitchenObjectsSO)) {
            //already has this type
            return false;
        }
        AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOsIndexFromList(kitchenObjectsSO));

        return true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOsIndex) {
        AddIngredientClientRpc(kitchenObjectSOsIndex);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOsIndex) {
        KitchenObjectsSO kitchenObjectsSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOsIndex);
        kitchenObjectsSOList.Add(kitchenObjectsSO);
        OnIngredeintAdded?.Invoke(this, new OnIngredeintAddedEventArgs {
            kitchenObjectSO = kitchenObjectsSO
        });
    }
    public List<KitchenObjectsSO> GetKitchenObjectsSOList() {
        return kitchenObjectsSOList;
    }
}