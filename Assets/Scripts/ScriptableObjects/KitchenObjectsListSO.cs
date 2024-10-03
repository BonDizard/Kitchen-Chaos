/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: to hold all kitchen objects(ingridents)
 */
using System.Collections.Generic;
using UnityEngine;
// [CreateAssetMenu()]
public class KitchenObjectsListSO : ScriptableObject {
    [SerializeField] public List<KitchenObjectsSO> kitchenObjectsSOsList;
}
