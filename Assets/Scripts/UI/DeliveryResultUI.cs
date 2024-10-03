/*
 * Author: Bharath Kumar S
 * Date: 27-09-2024
 * Description: Deliver Result UI
 */
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {
    private const string DELIVERY_RESULT_POPUP = "DeliveryResultPopup";
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image BackgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite successIconSprite;
    [SerializeField] private Sprite failerIconSprite;
    [SerializeField] private Color sucessColor;
    [SerializeField] private Color failedColor;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        DeliverManager.Instance.OnRecipeSucess += DeliverManager_OnRecipeSucess;
        DeliverManager.Instance.OnRecipeFailed += DeliverManager_OnRecipeFailed;
        gameObject.SetActive(false);
    }
    private void DeliverManager_OnRecipeSucess(object sender, System.EventArgs e) {
        Debug.LogWarning("DeliverManager_OnRecipeSucess");
        gameObject.SetActive(true);
        animator.SetTrigger(DELIVERY_RESULT_POPUP);
        iconImage.sprite = successIconSprite;
        BackgroundImage.color = sucessColor;
        messageText.text = "Develiry\nSucess";

    }
    private void DeliverManager_OnRecipeFailed(object sender, System.EventArgs e) {
        Debug.LogWarning("DeliverManager_OnRecipeFailed");
        gameObject.SetActive(true);
        animator.SetTrigger(DELIVERY_RESULT_POPUP);
        iconImage.sprite = failerIconSprite;
        BackgroundImage.color = failedColor;
        messageText.text = "Develiry\nFailed";
    }

}
