/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Menu UI 
 */
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button multiplayerPlayButton;
    [SerializeField] private Button singlePlayerPlayButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        multiplayerPlayButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.isMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        singlePlayerPlayButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.isMultiplayer = false;
            Loader.Load(Loader.Scene.GameScene);
        });
        Time.timeScale = 1f;
    }
}
