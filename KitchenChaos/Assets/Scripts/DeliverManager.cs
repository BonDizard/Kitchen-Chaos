/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Delivery Logic handled here.
 */
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliverManager : NetworkBehaviour {
    public static DeliverManager Instance { get; private set; }

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSucess;
    public event EventHandler OnRecipeFailed;

    [SerializeField] private RecipeListSO menu;
    private List<RecipeSO> waitingRecipeSOList;
    //dont want to spawn imediatly
    private float spawnRecipeTimer = 4f;
    private float spawnTimerMax = 4f;
    private int maxWaitingOrders = 4;
    private int successfulDeliveredItems;
    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnTimerMax;
            // Debug.Log("DeliverManager Update - Spawn Timer Reset: " + spawnTimerMax);

            if (KitchenGameManger.Instance.IsGamePlaying() && waitingRecipeSOList.Count < maxWaitingOrders) {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, menu.recipeList.Count);
                //now only server will call this 
                ShowNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
            else {
                // Debug.Log("Max waiting orders reached: " + maxWaitingOrders);
            }
        }
    }
    [ClientRpc]
    private void ShowNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = menu.recipeList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        // Debug.Log("Recipe Spawned: " + waitingRecipeSO.name + " | Total Waiting Recipes: " + waitingRecipeSOList.Count);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
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
                bool plateContentsMatchesRecipe = true;

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
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                // If all ingredients matched, complete the order
                if (plateContentsMatchesRecipe) {
                    DeliverCorrectRecipeServerRpc(i);
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
        DeliverInCorrectRecipeServerRpc();
        //Debug.LogWarning("No matching recipe found for the plate.");
    }
    //RequireOwnership = false to allow clients to call this
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex) {
        // Debug.Log("Correct recipe delivered: " + waitingRecipe.name);
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSucess?.Invoke(this, EventArgs.Empty);
        successfulDeliveredItems++;
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverInCorrectRecipeServerRpc() {
        DeliverInCorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverInCorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    public List<RecipeSO> GetWaitingRecipeSOList() {
        // Debug.Log("GetWaitingRecipeSOList called - Current waiting recipe count: " + waitingRecipeSOList.Count);
        return waitingRecipeSOList;
    }

    public int GetSuccessfulDeliveredItems() {
        return successfulDeliveredItems;
    }
}
