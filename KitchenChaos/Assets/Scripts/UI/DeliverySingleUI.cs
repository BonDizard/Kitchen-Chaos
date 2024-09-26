/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Logic for recipe template UI
 */
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverySingleUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake() {
        Debug.Log("DeliverySingleUI Awake - Icon template deactivated.");
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        Debug.Log("Setting Recipe in UI: " + recipeSO.name);
        recipeNameText.text = recipeSO.name;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Debug.Log("Destroying old icon.");
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectsSO kitchenObjectsSO in recipeSO.recipeList) {
            Debug.Log("Adding ingredient icon: " + kitchenObjectsSO.name);
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectsSO.sprite;
        }
    }
}
