/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Description: Count Down UI 
 */
using TMPro;
using UnityEngine;

public class GameStartCOuntDownUI : MonoBehaviour {
    private const string NUMBER_POP_ANIMATION_NAME = "NumberPopUp";
    private Animator animator;
    private int previousNumber;
    [SerializeField] TextMeshProUGUI countDownText;


    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        Hide();
    }

    private void KitchenGameManger_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsCountdownToStartActive()) {
            Show();
        }
        else {
            Hide();
        }
    }
    private void Update() {
        int countDownNumber = Mathf.CeilToInt(KitchenGameManger.Instance.GetCountdownToStartTimer());
        countDownText.text = countDownNumber.ToString();
        if (previousNumber != countDownNumber) {
            previousNumber = countDownNumber;
            animator.SetTrigger(NUMBER_POP_ANIMATION_NAME);
            SoundManager.Instance.PlayCountDownSound();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
