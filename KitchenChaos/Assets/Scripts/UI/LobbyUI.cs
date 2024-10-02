/*
 * Author: Bharath Kumar S
 * Date: 2-10-2024
 * Description: join and create lobby UI
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private CreateLobbyUI createLobbyUIGameObject;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField lobbyCodeField;
    private void Awake() {
        quickJoinButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.QuickJoin();
        });
        createLobbyButton.onClick.AddListener(() => {
            createLobbyUIGameObject.Show();
        });
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MenuScene);
        });
        joinCodeButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.JoinWithCode(lobbyCodeField.text);
        });
    }

}
