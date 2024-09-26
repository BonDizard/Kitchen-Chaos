/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * All inputs of the game are handled here.
 */

using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resumeButton;
    private void Awake() {
        menuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MenuScene);
        });
        resumeButton.onClick.AddListener(() => {
            KitchenGameManger.Instance.TogglePauseGame();
        });
    }
    private void Start() {
        KitchenGameManger.Instance.OnGamePaused += KitchenGameManger_OnGamePaused;
        KitchenGameManger.Instance.OnGameUnPaused += KitchenGameManger_OnGameUnPaused;
        Hide();
    }
    private void KitchenGameManger_OnGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }
    private void KitchenGameManger_OnGamePaused(object sender, System.EventArgs e) {
        Show();
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
