/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Scriptable Object for Recipe 
 */

using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
    public List<KitchenObjectsSO> recipeList;
    public string recipeName;
}
