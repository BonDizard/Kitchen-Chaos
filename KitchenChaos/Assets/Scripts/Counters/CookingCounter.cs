using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookingCounter : BaseCounter, IProgressBar {

    [SerializeField] public CookingRecipeSO[] cookingRecipesArray;
    [SerializeField] public BurningRecipeSO[] burningRecipesArray;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public event EventHandler<IProgressBar.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangeEventArgs {
        public State state;
    }

    private float fryingTimer;
    private float burningTimer;
    public enum State {
        Idle,
        Frying,
        Fried,
        Burnt,
    }

    private CookingRecipeSO cookingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private State state;

    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        if (HasKitchenObject()) {

            switch (state) {

                case State.Idle:
                    // Do nothing when idle
                    break;

                case State.Frying:
                    if (cookingRecipeSO != null) {
                        fryingTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                            progressNormalized = fryingTimer / cookingRecipeSO.maxCookingTime
                        });

                        if (fryingTimer > cookingRecipeSO.maxCookingTime) {
                            if (GetKitchenObject() != null) {
                                GetKitchenObject().DestroySelf();
                                KitchenObject.SpawnKitchenObect(cookingRecipeSO.outputKitchenObject, this);
                                state = State.Fried;
                                burningTimer = 0f;
                                burningRecipeSO = GetBurningRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());

                                OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                                    state = state
                                });
                            }
                            else {
                                Debug.LogError("GetKitchenObject() is null during frying.");
                            }
                        }
                    }
                    else {
                        Debug.LogError("cookingRecipeSO is null. Make sure it is set before entering Frying state.");
                    }
                    break;

                case State.Fried:
                    if (burningRecipeSO != null) {
                        burningTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                            progressNormalized = burningTimer / burningRecipeSO.maxBurningTime
                        });

                        if (burningTimer > burningRecipeSO.maxBurningTime) {
                            if (GetKitchenObject() != null) {
                                GetKitchenObject().DestroySelf();
                                KitchenObject.SpawnKitchenObect(burningRecipeSO.outputKitchenObject, this);
                                state = State.Burnt;

                                OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                                    state = state
                                });

                                OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                                    progressNormalized = 0f
                                });
                            }
                            else {
                                Debug.LogError("GetKitchenObject() is null during burning.");
                            }
                        }
                    }
                    else {
                        Debug.LogError("burningRecipeSO is null. Make sure it is set before entering Fried state.");
                    }
                    break;

                case State.Burnt:
                    // Do nothing when burnt
                    break;
            }
            Debug.Log(fryingTimer);
        }
    }

    public override void Interact(Player player) {
        if (HasKitchenObject()) {
            // The counter has some object
            if (!player.HasKitchenObject()) {
                // The player does not have anything
                GetKitchenObject().SetClearIKitchenObjectParent(player);
                state = State.Idle;

                OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                    state = state
                });
            }
            else {
                // The player is holding something
            }
        }
        else {
            // The counter is empty
            if (player.HasKitchenObject()) {
                // Player has an object
                if (IsTheObjectCuttable(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player is holding an object that is cookable
                    player.GetKitchenObject().SetClearIKitchenObjectParent(this);
                    cookingRecipeSO = GetCookingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    fryingTimer = 0f;
                    state = State.Frying;

                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                        state = state
                    });

                    if (cookingRecipeSO != null) {
                        OnProgressChanged?.Invoke(this, new IProgressBar.OnProgressChangedEventArgs {
                            progressNormalized = fryingTimer / cookingRecipeSO.maxCookingTime
                        });
                    }
                    else {
                        Debug.LogError("cookingRecipeSO is null after setting to Frying state.");
                    }
                }
            }
            else {
                // The player does not have an object
            }
        }
    }

    private bool IsTheObjectCuttable(KitchenObjectsSO inputKitchenObjectsSO) {
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
            if (burningRecipeSO.inputKitchenObject == inputKitchenObjectsSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }
}
