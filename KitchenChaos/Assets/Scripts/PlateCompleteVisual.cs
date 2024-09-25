/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Plate logic for full burger visual
 */

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {

    //custom datatypes wont be visible in the unity editor so add serializable
    [Serializable]
    public struct KitchenObjectsSO_GameObject {
        public KitchenObjectsSO kitchenObjectsSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectsSO_GameObject> kitchenObjectsSOGameObjectsList;
    private void Start() {
        plateKitchenObject.OnIngredeintAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitchenObjectsSO_GameObject kitchenObjectsSOGameObject in kitchenObjectsSOGameObjectsList) {
            kitchenObjectsSOGameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredeintAddedEventArgs e) {
        foreach (KitchenObjectsSO_GameObject kitchenObjectsSOGameObject in kitchenObjectsSOGameObjectsList) {
            if (kitchenObjectsSOGameObject.kitchenObjectsSO == e.kitchenObjectSO) {
                kitchenObjectsSOGameObject.gameObject.SetActive(true);
            }
        }
    }
}
