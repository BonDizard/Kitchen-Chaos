using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CookingRecipeSO : ScriptableObject {
    public KitchenObjectsSO inputKitchenObject;
    public KitchenObjectsSO outputKitchenObject;
    public float maxCookingTime;
}
