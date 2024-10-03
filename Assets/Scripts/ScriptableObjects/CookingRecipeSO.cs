/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Scriptable Object for Cooked Burger
 */
using UnityEngine;

[CreateAssetMenu()]
public class CookingRecipeSO : ScriptableObject {
    public KitchenObjectsSO inputKitchenObject;
    public KitchenObjectsSO outputKitchenObject;
    public float maxCookingTime;
}
