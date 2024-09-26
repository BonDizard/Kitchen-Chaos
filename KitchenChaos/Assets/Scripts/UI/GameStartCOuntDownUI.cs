/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Count Down UI 
 */
using TMPro;
using UnityEngine;

public class GameStartCOuntDownUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI countDownText;
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        Hide();
    }

    private void KitchenGameManger_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsInCountDownState()) {
            Show();
        }
        else {
            Hide();
        }
    }
    private void Update() {
        countDownText.text = Mathf.Ceil(KitchenGameManger.Instance.GetCountDownTime()).ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
