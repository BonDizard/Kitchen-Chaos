/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Cutting counter logic
 */
using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IProgressBar {
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData() {
        OnAnyCut = null;
    }
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
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
        }
        else {
            //the counter is empty
            if (player.HasKitchenObject()) {
                //player has an object
                if (IsTheObjectCuttable(player.GetKitchenObject().GetKitchenObjectSO())) {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    //player is having object which is cuttable
                    kitchenObject.SetKitchenObjectToParent(this);
                    InteractLogicPlaceObjectToCounterServerRpc();
                }
            }
            else {
                //the player is not having an object
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectToCounterServerRpc() {
        InteractLogicPlaceObjectToCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectToCounterClientRpc() {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
            progressNormalized = 0f
        });
    }
    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && IsTheObjectCuttable(GetKitchenObject().GetKitchenObjectSO())) {
            CuttingKitchenObjectServerRpc();
        }
    }

    [ServerRpc]
    private void CuttingKitchenObjectServerRpc() {
        CuttingKitchenObjectClientRpc();
        TestCuttingProgressDoneServerRpc();
    }
    [ClientRpc]
    private void CuttingKitchenObjectClientRpc() {
        //ther is kitchen object and alternate key is pressed then cut the object
        cuttingProgress++;

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCutRequiredForObject
        });

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);//for sound
    }
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc() {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

        if (cuttingProgress >= cuttingRecipeSO.maxCutRequiredForObject) {
            KitchenObjectsSO outputKitchenObjectSO = GetKitchenObjectFromInput(GetKitchenObject().GetKitchenObjectSO());
            //object present
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObect(outputKitchenObjectSO, this);
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
