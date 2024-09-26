/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Game Over UI 
 */
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI deliveredItems;
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        Hide();
    }

    private void KitchenGameManger_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsGameOver()) {
            deliveredItems.text = Mathf.Ceil(DeliverManager.Instance.GetSuccessfulDeliveredItems()).ToString();
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
