/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Recipie Icon template logic
 */
using UnityEngine;

public class PlateIconUI : MonoBehaviour {
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;
    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
    }
    private void Start() {
        plateKitchenObject.OnIngredeintAdded += PlateKitchenObject_OnIngridentsAdded;
    }

    private void PlateKitchenObject_OnIngridentsAdded(object sender, PlateKitchenObject.OnIngredeintAddedEventArgs e) {
        UpdateVisual();
    }
    private void UpdateVisual() {
        foreach (Transform child in transform) {
            if (child == iconTemplate) {
                continue;
            }
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectsSO kitchenObjectsSO in plateKitchenObject.GetKitchenObjectsSOList()) {
            //make sure to use transform to spawn the icon as child of this obnject
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectsSO);
        }
    }
}
