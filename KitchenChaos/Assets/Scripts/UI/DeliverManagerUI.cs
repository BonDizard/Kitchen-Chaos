/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Delivery UI Logic handled here.
 */
using UnityEngine;

public class DeliverManagerUI : MonoBehaviour {
    [SerializeField] Transform container;
    [SerializeField] Transform recipeTemplate;

    private void Awake() {
        Debug.Log("DeliverManagerUI Awake - Recipe template deactivated.");
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        Debug.Log("DeliverManagerUI Start - Subscribing to DeliverManager events.");
        DeliverManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliverManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
        Debug.Log("Recipe Spawned Event Triggered");
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
        Debug.Log("Recipe Completed Event Triggered");
        UpdateVisual();
    }

    private void UpdateVisual() {
        Debug.Log("Updating Delivery Manager UI");
        foreach (Transform child in container) {
            if (child == recipeTemplate) {
                continue;
            }
            Debug.Log("Destroying old recipe UI element.");
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipeSO in DeliverManager.Instance.GetWaitingRecipeSOList()) {
            Debug.Log("Creating UI for recipe: " + recipeSO.name);
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTemplate.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliverySingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
