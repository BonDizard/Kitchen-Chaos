/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Cooking counter logic
 */

using System;
using Unity.Netcode;
using UnityEngine;
public class CookingCounter : BaseCounter, IProgressBar {
    public enum State {
        Idle,
        Frying,
        Fried,
        Burnt,
    }

    public event EventHandler<IProgressBar.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs {
        public State state;
    }
    [SerializeField] public CookingRecipeSO[] cookingRecipesArray;
    [SerializeField] public BurningRecipeSO[] burningRecipesArray;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private CookingRecipeSO cookingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    public override void OnNetworkSpawn() {
        fryingTimer.OnValueChanged += FryingTime_OnValueCahanged;
        burningTimer.OnValueChanged += BuringTime_OnValueCahanged;
        state.OnValueChanged += State_OnValueChanged;
    }
    private void State_OnValueChanged(State oldState, State newstate) {
        OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
            state = state.Value
        });
        //dont show progress bar
        if (state.Value == State.Idle || state.Value == State.Burnt) {
            OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                progressNormalized = 0f
            });
        }
    }

    private void BuringTime_OnValueCahanged(float oldValue, float newvalue) {
        float burningTimeMax = burningRecipeSO != null ? burningRecipeSO.maxBurningTime : 1f;
        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
            progressNormalized = burningTimeMax
        });
    }
    private void FryingTime_OnValueCahanged(float oldValue, float newvalue) {
        float fryingTimeMax = cookingRecipeSO != null ? cookingRecipeSO.maxCookingTime : 1f;
        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
            progressNormalized = fryingTimeMax
        });
    }
    private void Update() {
        if (!IsServer) {
            return;
        }
        if (HasKitchenObject()) {
            switch (state.Value) {
                case State.Idle:
                    // Do nothing when idle
                    break;

                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;
                    if (fryingTimer.Value > cookingRecipeSO.maxCookingTime) {
                        //fried
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObect(cookingRecipeSO.outputKitchenObject, this);
                        // Debug.Log("Spawned: " + cookingRecipeSO.outputKitchenObject);
                        // Debug.Log("GetKitchenObject(): " + GetKitchenObject());
                        // Debug.Log("GetKitchenObject().GetKitchenObjectSO(): " + GetKitchenObject().GetKitchenObjectSO());

                        state.Value = State.Fried;
                        burningTimer.Value = 0f;
                        SetBuringRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOsIndexFromList(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;
                    if (burningTimer.Value > burningRecipeSO.maxBurningTime) {

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObect(burningRecipeSO.outputKitchenObject, this);
                        state.Value = State.Burnt;
                    }
                    break;
                case State.Burnt:
                    // Do nothing when burnt
                    break;
            }
            //Debug.Log(fryingTimer);
        }
    }

    public override void Interact(Player player) {
        if (HasKitchenObject()) {
            // The counter has some object
            if (!player.HasKitchenObject()) {
                // The player does not have anything
                GetKitchenObject().SetKitchenObjectToParent(player);
                state.Value = State.Idle;
            }
            else {
                // The player is holding something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        SetStateIdleServerRpc();
                    }
                }
            }
        }
        else {
            // The counter is empty
            if (player.HasKitchenObject()) {
                // Player has an object
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player is holding an object that is cookable
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectToParent(this);
                    int kitchenObjectSOsIndex = KitchenGameMultiplayer.Instance.GetKitchenObjectSOsIndexFromList(kitchenObject.GetKitchenObjectSO());
                    SetFringRecipeSOServerRpc(kitchenObjectSOsIndex);
                }
            }
            else {
                // The player does not have an object
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        SetStateIdleClientRpc();
    }
    [ClientRpc]
    private void SetStateIdleClientRpc() {
        state.Value = State.Idle;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetFringRecipeSOServerRpc(int kitchenObjectSOsIndex) {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
        SetFringRecipeSOClientRpc(kitchenObjectSOsIndex);
    }
    [ClientRpc]
    private void SetFringRecipeSOClientRpc(int kitchenObjectSOsIndex) {
        KitchenObjectsSO kitchenObjectsSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOsIndex);
        cookingRecipeSO = GetCookingRecipeSOFromInput(kitchenObjectsSO);
    }
    [ClientRpc]
    private void SetBuringRecipeSOClientRpc(int kitchenObjectSOsIndex) {
        KitchenObjectsSO kitchenObjectsSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOsIndex);
        burningRecipeSO = GetBurningRecipeSOFromInput(kitchenObjectsSO);
    }
    private bool HasRecipeWithInput(KitchenObjectsSO inputKitchenObjectsSO) {
        CookingRecipeSO cookingRecipeSO = GetCookingRecipeSOFromInput(inputKitchenObjectsSO);
        return cookingRecipeSO != null;
    }

    private KitchenObjectsSO GetKitchenObjectFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        CookingRecipeSO cookingRecipeSO = GetCookingRecipeSOFromInput(inputKitchenObjectsSO);
        if (cookingRecipeSO != null) {
            return cookingRecipeSO.outputKitchenObject;
        }
        else {
            return null;
        }
    }

    private CookingRecipeSO GetCookingRecipeSOFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        foreach (CookingRecipeSO cookingRecipeSO in cookingRecipesArray) {
            if (cookingRecipeSO.inputKitchenObject == inputKitchenObjectsSO) {
                return cookingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOFromInput(KitchenObjectsSO inputKitchenObjectsSO) {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipesArray) {

            // Debug.Log("inputKitchenObjectsSO: " + inputKitchenObjectsSO.name);
            // Debug.Log("burningRecipeSO: " + burningRecipeSO.inputKitchenObject.name);
            if (burningRecipeSO.inputKitchenObject == inputKitchenObjectsSO) {
                return burningRecipeSO;
            }
        }
        Debug.LogError("-----GetBurningRecipeSOFromInput returning null-----");
        return null;
    }
    public bool IsFried() {
        return state.Value == State.Fried;
    }
}
