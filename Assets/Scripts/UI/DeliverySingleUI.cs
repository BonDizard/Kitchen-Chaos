/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
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
        iconTemplate.gameObject.SetActive(false);
    }
    public void SetRecipeSO(RecipeSO recipeSO) {

        // Check if recipeSO is null
        if (recipeSO == null) {
            // Debug.LogError("recipeSO is null! Cannot set recipe.");
            return;
        }

        // Check if recipeSO.recipeList is null
        if (recipeSO.recipeList == null) {
            // Debug.LogError("recipeSO.recipeList is null! No ingredients to display.");
            return;
        }

        // Check if recipeSO.recipeList is empty
        if (recipeSO.recipeList.Count == 0) {
            // Debug.LogWarning("recipeSO.recipeList is empty! No ingredients to display.");
            return;
        }

        recipeNameText.text = recipeSO.name;

        // Clean up previous icons
        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        // Instantiate icons for the new recipe
        foreach (KitchenObjectsSO kitchenObjectsSO in recipeSO.recipeList) {
            // Debug.Log("Attempting to add icon for ingredient: " + kitchenObjectsSO.name);

            // Instantiate the icon from the template
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            if (iconTransform == null) {
                // Debug.LogError("Icon instantiation failed for ingredient: " + kitchenObjectsSO.name);
                continue;
            }

            // Activate the instantiated icon object
            iconTransform.gameObject.SetActive(true);
            // Debug.Log("Icon activated for ingredient: " + kitchenObjectsSO.name);

            // Set the sprite for the icon
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null) {
                iconImage.sprite = kitchenObjectsSO.sprite;
                // Debug.Log("Icon sprite set for ingredient: " + kitchenObjectsSO.name);

                if (iconImage.sprite == null) {
                    Debug.LogError("<<<<<Icon code error>>>>>>>>>>>>>>> Sprite missing for ingredient: " + kitchenObjectsSO.name);
                }
            }
            else {
                Debug.LogError("<<<<<Icon code error>>>>>>>>>>>>>>> Image component missing on icon for ingredient: " + kitchenObjectsSO.name);
            }
        }
    }
}
