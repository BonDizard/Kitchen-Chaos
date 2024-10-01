/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Cutting counter logic
 */
using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData() {
        OnAnyCut = null;
    }
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    private int cuttingProgress;

    public override void Interact(Player player) {
        Debug.Log("[CuttingCounter] Interact triggered by player.");

        if (HasKitchenObject()) {
            Debug.Log("[CuttingCounter] Counter has an object.");
            // The counter has some object
            if (!player.HasKitchenObject()) {
                Debug.Log("[CuttingCounter] Player has no object, giving kitchen object to player.");
                // Player does not have anything
                GetKitchenObject().SetKitchenObjectToParent(player);
            }
            else {
                Debug.Log("[CuttingCounter] Player has an object.");
                // Player has something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    Debug.Log("[CuttingCounter] Player has a plate, trying to add ingredients.");
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO())) {
                        // Place the cut object in the plate and remove it from the counter
                        Debug.Log("[CuttingCounter] Ingredient added to plate, destroying kitchen object on counter.");
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
        }
        else {
            Debug.Log("[CuttingCounter] Counter is empty.");
            // The counter is empty
            if (player.HasKitchenObject()) {
                Debug.Log("[CuttingCounter] Player has an object.");
                // Player has an object
                if (IsTheObjectCuttable(player.GetKitchenObject().GetKitchenObjectSO())) {
                    Debug.Log("[CuttingCounter] Player's object is cuttable, placing on counter.");
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    // Player is holding an object that is cuttable
                    kitchenObject.SetKitchenObjectToParent(this);
                    InteractLogicPlaceObjectToCounterServerRpc();
                }
            }
            else {
                Debug.Log("[CuttingCounter] Player does not have an object.");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectToCounterServerRpc() {
        Debug.Log("[CuttingCounter] Placing object on counter (Server RPC).");
        InteractLogicPlaceObjectToCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectToCounterClientRpc() {
        Debug.Log("[CuttingCounter] Placing object on counter (Client RPC). Resetting cutting progress.");
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = 0f
        });
    }

    public override void InteractAlternate(Player player) {
        Debug.Log("[CuttingCounter] Alternate interaction triggered.");
        if (HasKitchenObject() && IsTheObjectCuttable(GetKitchenObject().GetKitchenObjectSO())) {
            Debug.Log("[CuttingCounter] Object is cuttable, starting cutting process.");
            CuttingKitchenObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CuttingKitchenObjectServerRpc() {
        Debug.Log("[CuttingCounter] Cutting object (Server RPC).");
        CuttingKitchenObjectClientRpc();
    }

    [ClientRpc]
    private void CuttingKitchenObjectClientRpc() {
        Debug.Log("[CuttingCounter] Cutting object (Client RPC). Current progress: " + cuttingProgress);
        // There is a kitchen object and the alternate key is pressed, cut the object
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty); // For sound

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
        float progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCutRequiredForObject;

        Debug.Log("[CuttingCounter] Progress updated: " + progressNormalized);

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = progressNormalized
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc() {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

        if (cuttingProgress >= cuttingRecipeSO.maxCutRequiredForObject) {
            Debug.Log("[CuttingCounter] Cutting progress done. Transforming object.");
            KitchenObjectsSO outputKitchenObjectSO = GetKitchenObjectFromInput(GetKitchenObject().GetKitchenObjectSO());
            // Object is done being cut
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObect(outputKitchenObjectSO, this);
        }
    }

    private bool IsTheObjectCuttable(KitchenObjectsSO inputKitchenObjectsSO) {
        Debug.Log("[CuttingCounter] Checking if object is cuttable: " + inputKitchenObjectsSO.name);
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectsSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectsSO GetKitchenObjectFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(inputKitchenObjectsSO);
        if (cuttingRecipeSO != null) {
            Debug.Log("[CuttingCounter] Object is cuttable, output object: " + cuttingRecipeSO.outputKitchenObject.name);
            return cuttingRecipeSO.outputKitchenObject;
        }
        else {
            Debug.Log("[CuttingCounter] Object is not cuttable.");
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.inputKitchenObject == inputKitchenObjectsSO) {
                Debug.Log("[CuttingCounter] Found matching cutting recipe for: " + inputKitchenObjectsSO.name);
                return cuttingRecipeSO;
            }
        }
        Debug.Log("[CuttingCounter] No matching cutting recipe found for: " + inputKitchenObjectsSO.name);
        return null;
    }
}
