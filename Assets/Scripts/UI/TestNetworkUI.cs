/*
 * Author: Bharath Kumar S
 * Date: 27-09-2024
 * Description: starting host clinet logic
 */
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetworkUI : MonoBehaviour {
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    private void Start() {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("HOST");
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            Debug.Log("CLIENT");
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
