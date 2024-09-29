/*
 * Author: Bharath Kumar S
 * Date: 29-09-2024
 * Description: multiplayer pasue ui logic
 */

using UnityEngine;

public class MultiplayerGamePauseUI : MonoBehaviour {
    private void Start() {
        KitchenGameManger.Instance.OnMultiPlayerGamePaused += KitchenGameManger_OnMultiPlayerGamePaused;
        KitchenGameManger.Instance.OnMultiPlayerGameUnPaused += KitchenGameManger_OnMultiPlayerGameUnPaused;
        Hide();
    }
    private void KitchenGameManger_OnMultiPlayerGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }
    private void KitchenGameManger_OnMultiPlayerGamePaused(object sender, System.EventArgs e) {
        Show();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show() {
        gameObject.SetActive(true);

    }

}
