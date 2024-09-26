/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Scriptable Object for Recipe List (Which will hold menu)
 */
using System.Collections.Generic;
using UnityEngine;
// [CreateAssetMenu()]
public class RecipeListSO : ScriptableObject {
    public List<RecipeSO> recipeList;
}
