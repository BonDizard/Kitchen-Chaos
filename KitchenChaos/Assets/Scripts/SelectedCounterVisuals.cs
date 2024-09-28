/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Changing the counter visuals when the player is in front of it.
 */
using UnityEngine;

public class SelectedCounterVisuals : MonoBehaviour {
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start() {
        // Subscribe to the event (listening) and trigger Player_OnSelectedCounterChange when the event is fired
        // Player.Instance.onSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(object sender, Player.onSelectedCounterChangeEventArgs e) {
        if (e.selectedCounter == baseCounter) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}
