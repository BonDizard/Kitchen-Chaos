/*
 * Author: Bharath Kumar S
 * Date: 29-09-2024
 * Descriptions: Waiting for players UI logic
 */
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour {

    // Key to manually toggle the visibility of the UI for debugging purposes
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        KitchenGameManger.Instance.OnLocalPlayerReadyChanged += KitchenGameManger_OnLocalPlayerReadyChanged;
        Hide();
    }

    private void KitchenGameManger_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void KitchenGameManger_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsLocalPlayerReady()) {
            Show();
        }
    }

    private void Show() {
        Debug.Log("[WaitingForPlayersUI] Showing UI.");
        gameObject.SetActive(true);
    }

    private void Hide() {
        Debug.Log("[WaitingForPlayersUI] Hiding UI.");
        gameObject.SetActive(false);
    }
}
