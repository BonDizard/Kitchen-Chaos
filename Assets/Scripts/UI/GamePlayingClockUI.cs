/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Clock while playing UI
 */
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {
    [SerializeField] private Image timerImage;

    private void Update() {
        timerImage.fillAmount = KitchenGameManger.Instance.GetGamePlayingTimerNormalized();
    }
}
