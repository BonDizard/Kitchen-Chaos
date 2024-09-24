using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookingCounter : BaseCounter {
    [SerializeField] public CookingRecipeSO[] cookingRecipesArray;
    [SerializeField] public BurningRecipeSO[] burningRecipesArray;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;

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
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    if (fryingTimer > cookingRecipeSO.maxCookingTime) {

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObect(cookingRecipeSO.outputKitchenObject, this);
                        Debug.Log("Fried!");
                        state = State.Fried;

                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                            state = state
                        });

                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    if (burningTimer > burningRecipeSO.maxBurningTime) {

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObect(burningRecipeSO.outputKitchenObject, this);
                        Debug.Log("Burnt!");
                        state = State.Burnt;

                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                            state = state
                        });

                    }
                    break;
                case State.Burnt:
                    break;
            }
            Debug.Log(fryingTimer);
        }
    }

    public override void Interact(PlayerScript player) {
        if (HasKitchenObject()) {
            //the counter has some object
            if (!player.HasKitchenObject()) {
                //the player not have anything
                GetKitchenObject().SetClearIKitchenObjectParent(player);
                state = State.Idle;

                OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                    state = state
                });
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
                    //player is having object which is cookable
                    player.GetKitchenObject().SetClearIKitchenObjectParent(this);
                    cookingRecipeSO = GetCookingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    fryingTimer = 0f;
                    state = State.Frying;

                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs {
                        state = state
                    });
                }
            }
            else {
                //the player is not having an object
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