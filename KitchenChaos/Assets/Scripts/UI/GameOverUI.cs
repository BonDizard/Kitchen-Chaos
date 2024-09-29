/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Game Over UI 
 */
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI deliveredItems;
    [SerializeField] Button PlayAgainButton;
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        PlayAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MenuScene);
        });
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
