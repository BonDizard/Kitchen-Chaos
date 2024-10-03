/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Scriptable Object for Cutting ingredients
 */
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject {
    public KitchenObjectsSO inputKitchenObject;
    public KitchenObjectsSO outputKitchenObject;
    public int maxCutRequiredForObject;
}
