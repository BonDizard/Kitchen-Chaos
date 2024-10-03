/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Scriptable Object for Burned Burger
 */
using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject {
    public KitchenObjectsSO inputKitchenObject;
    public KitchenObjectsSO outputKitchenObject;
    public float maxBurningTime;
}
