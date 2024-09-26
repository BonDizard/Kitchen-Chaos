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
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;
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
            Hide();
        });
    }
    private void Start() {
        KitchenGameManger.Instance.OnGameUnPaused += KitchenGameManger_OnGameUnPaused;
        UpdateVisual();
        Hide();
    }
    private void KitchenGameManger_OnGameUnPaused(object sender, EventArgs e) {
        Hide();
    }
    private void UpdateVisual() {
        soundEffectText.text = "Sound Effects: " + Math.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music Effects: " + Math.Round(MusicManger.Instance.GetVolume() * 10f);
    }
    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
