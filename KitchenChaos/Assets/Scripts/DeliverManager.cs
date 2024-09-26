/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Delivery Logic handled here.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliverManager : MonoBehaviour {
    public static DeliverManager Instance { get; private set; }
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public RecipeListSO menu;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnTimer;
    private float spawnTimerMax = 4f;
    private int maxWaitingOrders = 4;

    private void Awake() {
        Debug.Log("DeliverManager Awake - Initializing Instance and Waiting Recipe List");
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f) {
            spawnTimer = spawnTimerMax;
            Debug.Log("DeliverManager Update - Spawn Timer Reset: " + spawnTimerMax);

            if (waitingRecipeSOList.Count < maxWaitingOrders) {
                RecipeSO waitingRecipeSO = menu.recipeList[UnityEngine.Random.Range(0, menu.recipeList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                Debug.Log("Recipe Spawned: " + waitingRecipeSO.name + " | Total Waiting Recipes: " + waitingRecipeSOList.Count);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
            else {
                Debug.Log("Max waiting orders reached: " + maxWaitingOrders);
            }
        }
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject) {
        Debug.Log("DeliveryRecipe Called");
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipe = waitingRecipeSOList[i];
            Debug.Log("Checking Recipe: " + waitingRecipe.name);

            if (plateKitchenObject.GetKitchenObjectsSOList().Count == waitingRecipe.recipeList.Count) {
                bool orderMatch = true;

                foreach (KitchenObjectsSO plateKitchenObjectsSO in plateKitchenObject.GetKitchenObjectsSOList()) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectsSO recipeKitchenObjectsSO in waitingRecipe.recipeList) {
                        if (plateKitchenObject == recipeKitchenObjectsSO) {
                            ingredientFound = true;
                            Debug.Log("Ingredient matched: " + plateKitchenObjectsSO.name);
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        orderMatch = false;
                        Debug.Log("Ingredient not matched: " + plateKitchenObjectsSO.name);
                        break;
                    }
                }

                if (orderMatch) {
                    Debug.Log("Correct recipe delivered: " + waitingRecipe.name);
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
                else {
                    Debug.Log("Recipe not matched.");
                }
            }
            else {
                Debug.Log("Recipe ingredient count mismatch.");
            }
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        Debug.Log("GetWaitingRecipeSOList called - Current waiting recipe count: " + waitingRecipeSOList.Count);
        return waitingRecipeSOList;
    }
}
