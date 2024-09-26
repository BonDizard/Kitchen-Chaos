/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Delivery Logic handled here.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliverManager : MonoBehaviour {
    public static DeliverManager Instance { get; private set; }
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSucess;
    public event EventHandler OnRecipeFailed;

    public RecipeListSO menu;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnTimer;
    private float spawnTimerMax = 4f;
    private int maxWaitingOrders = 4;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f) {
            spawnTimer = spawnTimerMax;
            // Debug.Log("DeliverManager Update - Spawn Timer Reset: " + spawnTimerMax);

            if (waitingRecipeSOList.Count < maxWaitingOrders) {
                RecipeSO waitingRecipeSO = menu.recipeList[UnityEngine.Random.Range(0, menu.recipeList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                // Debug.Log("Recipe Spawned: " + waitingRecipeSO.name + " | Total Waiting Recipes: " + waitingRecipeSOList.Count);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
            else {
                Debug.Log("Max waiting orders reached: " + maxWaitingOrders);
            }
        }
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject) {
        // Debug.Log("Attempting to deliver a recipe...");

        // Loop through each waiting recipe
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipe = waitingRecipeSOList[i];
            // Debug.Log("Checking waiting recipe: " + waitingRecipe.name);

            // Check if ingredient count matches between the plate and the recipe
            int plateIngredientCount = plateKitchenObject.GetKitchenObjectsSOList().Count;
            int recipeIngredientCount = waitingRecipe.recipeList.Count;
            // Debug.Log("Plate ingredient count: " + plateIngredientCount + ", Recipe ingredient count: " + recipeIngredientCount);

            if (plateIngredientCount == recipeIngredientCount) {
                bool orderMatch = true;

                // Loop through each ingredient in the plate
                foreach (KitchenObjectsSO plateKitchenObjectsSO in plateKitchenObject.GetKitchenObjectsSOList()) {
                    // Debug.Log("Checking plate ingredient: " + plateKitchenObjectsSO.name);

                    bool ingredientFound = false;

                    // Loop through each ingredient in the waiting recipe
                    foreach (KitchenObjectsSO recipeKitchenObjectsSO in waitingRecipe.recipeList) {
                        // Debug.Log("Comparing with recipe ingredient: " + recipeKitchenObjectsSO.name);

                        // Check if ingredients match
                        if (plateKitchenObjectsSO == recipeKitchenObjectsSO) {
                            ingredientFound = true;
                            // Debug.Log("Ingredient match found: " + plateKitchenObjectsSO.name);
                            break;
                        }
                    }

                    if (!ingredientFound) {
                        // Debug.LogWarning("Ingredient not found in recipe: " + plateKitchenObjectsSO.name);
                        orderMatch = false;
                        break;
                    }
                }

                // If all ingredients matched, complete the order
                if (orderMatch) {
                    // Debug.Log("Correct recipe delivered: " + waitingRecipe.name);
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSucess?.Invoke(this, EventArgs.Empty);

                    return;
                }
                else {
                    Debug.LogWarning("Recipe did not match: " + waitingRecipe.name);
                }
            }
            else {
                Debug.LogWarning("Ingredient count mismatch for recipe: " + waitingRecipe.name);
            }
        }
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        //Debug.LogWarning("No matching recipe found for the plate.");
    }


    public List<RecipeSO> GetWaitingRecipeSOList() {
        // Debug.Log("GetWaitingRecipeSOList called - Current waiting recipe count: " + waitingRecipeSOList.Count);
        return waitingRecipeSOList;
    }
}
