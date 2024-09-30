/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character ready UI
 */
using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterReadyUI : MonoBehaviour {
    [SerializeField] private Button readyButton;
    private void Awake() {
        readyButton.onClick.AddListener(() => {
            CharacterSetReady.Instance.SetPlayerReady();
        });
    }
}
