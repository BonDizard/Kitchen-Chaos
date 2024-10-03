/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Scriptable Object for Kitchen Objects(Ingridents)
 */
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectsSO : ScriptableObject {
    public string objectName;
    public Transform prefab;
    public Sprite sprite;
}