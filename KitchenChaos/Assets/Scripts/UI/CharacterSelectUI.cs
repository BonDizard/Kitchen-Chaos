/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character Selection UI
 */
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });
        readyButton.onClick.AddListener(() => {
            CharacterSetReady.Instance.SetPlayerReady();
        });
    }
}
