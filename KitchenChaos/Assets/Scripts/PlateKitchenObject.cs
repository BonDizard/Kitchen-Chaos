/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: plate objects behaviour
 */

using UnityEngine;
using System.Collections.Generic;
using System;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredeintAddedEventArgs> OnIngredeintAdded;
    public class OnIngredeintAddedEventArgs : EventArgs {
        public KitchenObjectsSO kitchenObjectSO;
    }
    List<KitchenObjectsSO> kitchenObjectsSOList;
    [SerializeField] private List<KitchenObjectsSO> validKitchenObjectSOList;
    private void Awake() {
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
        kitchenObjectsSOList.Add(kitchenObjectsSO);
        OnIngredeintAdded?.Invoke(this, new OnIngredeintAddedEventArgs {
            kitchenObjectSO = kitchenObjectsSO
        });
        return true;
    }

    public List<KitchenObjectsSO> GetKitchenObjectsSOList() {
        return kitchenObjectsSOList;
    }
}