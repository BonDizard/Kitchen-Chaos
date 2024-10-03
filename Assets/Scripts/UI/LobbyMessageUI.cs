/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Connecting UI
 */
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;
    private void Awake() {
        closeButton.onClick.AddListener(Hide);
    }
    private void Start() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToConnect;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;

        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnQuickJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;

        Hide();
    }
    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs e) {
        ShowMessage("Joining failed");
    }
    private void KitchenGameLobby_OnQuickJoinStarted(object sender, EventArgs e) {
        ShowMessage("Joining lobby......");
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs e) {
        ShowMessage("Quick join Failed");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs e) {
        ShowMessage("Creating lobby......");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e) {
        ShowMessage("Failed to create lobby");
    }

    private void KitchenGameMultiplayer_OnFailedToConnect(object sender, EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to connect");
        }
        else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }
    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToConnect;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;

        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnQuickJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
    }
}
