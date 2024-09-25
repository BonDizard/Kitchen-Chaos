using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject
{
    public KitchenObjectsSO inputKitchenObject;
    public KitchenObjectsSO outputKitchenObject;
    public float maxBurningTime;
}
