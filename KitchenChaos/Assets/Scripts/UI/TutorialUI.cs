/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Descriptions: Tutorial UI logic
 */
using System;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI KeyAlternateInteractText;
    [SerializeField] private TextMeshProUGUI KeyPauseText;
    [SerializeField] private TextMeshProUGUI KeyGamepadMoveText;
    [SerializeField] private TextMeshProUGUI KeyGamepadInteractText;
    [SerializeField] private TextMeshProUGUI KeyGamepadAlternateText;
    [SerializeField] private TextMeshProUGUI KeyGamepadPauseText;
    private void Start() {
        UpdateVisual();
        GameInput.Instance.OnBingingRebind += GameInput_OnBindingRebind;
        KitchenGameManger.Instance.OnStateChanged += KitchenGameManger_OnStateChanged;
        Show();
    }
    private void KitchenGameManger_OnStateChanged(object sender, EventArgs e) {
        Hide();
    }
    private void GameInput_OnBindingRebind(object sender, EventArgs e) {
        UpdateVisual();
    }
    private void UpdateVisual() {
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyAlternateInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyGamepadMoveText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyGamepadAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        KeyGamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);

    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show() {
        gameObject.SetActive(true);
    }
}
