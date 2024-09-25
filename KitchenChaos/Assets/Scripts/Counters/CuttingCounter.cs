/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Cutting counter logic
 */
using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IProgressBar {
    [SerializeField] private CuttingRecipeSO[] cuttingKitchenObjectsArray;
    public event EventHandler OnCut;
    public event EventHandler<IProgressBar.OnProgressChangedEventArgs> OnProgressChanged;

    private int cuttingProgress;
    public override void Interact(Player player) {
        //if there is onject player pickup
        if (HasKitchenObject()) {
            //the counter has some object
            if (!player.HasKitchenObject()) {
                //the player not have anything
                GetKitchenObject().SetKitchenObjectToParent(player);
            }
            else {
                //the player is having something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO())) {
                        //place the oblect that is cut in plate and delete in the cut table
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
        }
        else {
            //the counter is empty
            if (player.HasKitchenObject()) {
                //player has an object
                if (IsTheObjectCuttable(player.GetKitchenObject().GetKitchenObjectSO())) {
                    //player is having object which is cuttable
                    player.GetKitchenObject().SetKitchenObjectToParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCutRequiredForObject
                    });
                }
            }
            else {
                //the player is not having an object
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && IsTheObjectCuttable(GetKitchenObject().GetKitchenObjectSO())) {
            //ther is kitchen object and alternate key is pressed then cut the object
            cuttingProgress++;

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCutRequiredForObject
            });

            OnCut?.Invoke(this, EventArgs.Empty);

            if (cuttingProgress >= cuttingRecipeSO.maxCutRequiredForObject) {
                KitchenObjectsSO outputKitchenObjectSO = GetKitchenObjectFromInput(GetKitchenObject().GetKitchenObjectSO());
                //object present
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObect(outputKitchenObjectSO, this);
            }
        }
    }
    private bool IsTheObjectCuttable(KitchenObjectsSO inputKitchenObjectsSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectsSO);
        return cuttingRecipeSO != null;
    }
    private KitchenObjectsSO GetKitchenObjectFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectsSO);
        if (cuttingRecipeSO != null) {
            return cuttingRecipeSO.outputKitchenObject;
        }
        else {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingKitchenObjectsArray) {
            if (cuttingRecipeSO.inputKitchenObject == inputKitchenObjectsSO) {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
