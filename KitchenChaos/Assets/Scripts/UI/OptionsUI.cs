/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Descriptions Option UI logic
 */
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
    public static OptionsUI Instance { get; private set; }
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button alternateInteractButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI alternateText;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private Transform PressAKeyToRebind;
    private Action onCloseAction;
    private void Awake() {
        Instance = this;
        soundEffectButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManger.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => {
            onCloseAction();
            Hide();
        });
        moveUpButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Up);
        });
        moveDownButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Down);
        });
        moveLeftButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Left);
        });
        moveRightButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Right);
        });
        interactButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Interact);
        });
        alternateInteractButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Alternate_Interact);
        });
        pauseButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Pause);
        });
    }
    private void Start() {
        KitchenGameManger.Instance.OnLocalGameUnpaused += KitchenGameManger_OnGameUnPaused;
        UpdateVisual();
        Hide();
        HidePressAKeyToRebind();
    }
    private void KitchenGameManger_OnGameUnPaused(object sender, EventArgs e) {
        Hide();
    }
    private void UpdateVisual() {
        soundEffectText.text = "Sound Effects: " + Math.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music Effects: " + Math.Round(MusicManger.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        alternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Alternate_Interact);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }
    public void Show(Action onCloseAction) {
        this.onCloseAction = onCloseAction;
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
    private void ShowPressAKeyToRebind() {
        PressAKeyToRebind.gameObject.SetActive(true);
    }
    private void HidePressAKeyToRebind() {
        PressAKeyToRebind.gameObject.SetActive(false);
    }
    private void RebindBinding(GameInput.Binding binding) {
        ShowPressAKeyToRebind();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressAKeyToRebind();
            UpdateVisual();
        });
    }
}
