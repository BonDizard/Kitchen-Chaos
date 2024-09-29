/*
 * Author: Bharath Kumar S
 * Date: 29-09-2024
 * Descriptions: Waiting for players UI logic
 */
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour {
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        KitchenGameManger.Instance.OnLocalPlayerReadyChanged += KitchenGameManger_OnLocalPlayerReadyChanged;
        Show();
    }
    private void KitchenGameManger_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsInCountDownState()) {
            Hide();
        }
    }
    private void KitchenGameManger_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.GetIfLocalPlayerRedy()) {
            Hide();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
