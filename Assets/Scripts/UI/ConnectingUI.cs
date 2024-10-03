/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Connecting UI
 */

using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {
    private void Start() {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToConnect;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToConnect;
        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToConnect(object sender, EventArgs e) {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToConnect(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToConnect;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToConnect;
    }
}
