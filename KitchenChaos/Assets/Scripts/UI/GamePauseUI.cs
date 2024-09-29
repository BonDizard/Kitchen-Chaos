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
    [SerializeField] private Button optionsButton;

    private void Awake() {
        menuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MenuScene);
        });
        resumeButton.onClick.AddListener(() => {
            KitchenGameManger.Instance.TogglePauseGame();
        });
        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show(Show);
            Hide();
        });
    }
    private void Start() {
        KitchenGameManger.Instance.OnLocalGamePaused += KitchenGameManger_OnLocalGamePaused;
        KitchenGameManger.Instance.OnLocalGameUnPaused += KitchenGameManger_OnLocalGameUnPaused;
        Hide();
    }
    private void KitchenGameManger_OnLocalGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }
    private void KitchenGameManger_OnLocalGamePaused(object sender, System.EventArgs e) {
        Show();
    }
    private void Show() {
        gameObject.SetActive(true);

        resumeButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
