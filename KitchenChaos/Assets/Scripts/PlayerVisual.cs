/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Player visual
 */
using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    private Material material;
    private void Awake() {
        material = new Material(headMeshRenderer.material);
        headMeshRenderer.material = material;
        bodyMeshRenderer.material = material;
    }
    public void SetPlayerColor(Color color) {
        material.color = color;
    }
}
