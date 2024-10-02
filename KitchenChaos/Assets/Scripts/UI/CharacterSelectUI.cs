/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character Selection UI
 */
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });
        readyButton.onClick.AddListener(() => {
            CharacterSetReady.Instance.SetPlayerReady();
        });
    }
    private void Start() {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        lobbyName.text = "Lobby name: " + lobby.Name;
        lobbyCode.text = "Lobby code: " + lobby.LobbyCode;
    }
}
