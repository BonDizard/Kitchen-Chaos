/*
 * Author: Bharath Kumar S
 * Date: 29-09-2024
 * Description: Game Manger 
 */

using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour {
    [SerializeField] private Button playAgainButton;
    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });
    }
    void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        Hide();
    }
    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            //server shutting down
            Show();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }
}
