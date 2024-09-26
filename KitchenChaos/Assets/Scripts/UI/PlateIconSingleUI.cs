/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: single Icon template logic
 */
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour {
    [SerializeField] private Image image;
    public void SetKitchenObjectSO(KitchenObjectsSO kitchenObjectsSO) {
        image.sprite = kitchenObjectsSO.sprite;
    }
}
