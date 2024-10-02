/*
 * Author: Bharath Kumar S
 * Date: 2-10-2024
 * Description: Creating a lobby UI
 */
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour {
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameField;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
        createPublicButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreatLobby(lobbyNameField.text, false);
        });
        createPrivateButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreatLobby(lobbyNameField.text, true);
        });
    }

    public void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
