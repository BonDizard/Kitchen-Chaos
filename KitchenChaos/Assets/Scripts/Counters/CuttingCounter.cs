using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter, IProgressBar {
    [SerializeField] private CuttingRecipeSO[] cuttingKitchenObjectsArray;
    public event EventHandler OnCut;
    public event EventHandler<IProgressBar.OnProgressChangedEventArgs> OnProgressChanged;

    private int cuttingProgress;
    public override void Interact(PlayerScript player) {
        if (HasKitchenObject()) {
            //the counter has some object
            if (!player.HasKitchenObject()) {
                //the player not have anything
                GetKitchenObject().SetClearIKitchenObjectParent(player);
            }
            else {
                //the player is having something
            }
        }
        else {
            //the counter is empty
            if (player.HasKitchenObject()) {
                //player has an object
                if (IsTheObjectCuttable(player.GetKitchenObject().GetKitchenObjectSO())) {
                    //player is having object which is cuttable
                    player.GetKitchenObject().SetClearIKitchenObjectParent(this);
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                    cuttingProgress = 0;

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

    public override void InteractAlternate(PlayerScript player) {
        if (HasKitchenObject() && IsTheObjectCuttable(GetKitchenObject().GetKitchenObjectSO())) {

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
