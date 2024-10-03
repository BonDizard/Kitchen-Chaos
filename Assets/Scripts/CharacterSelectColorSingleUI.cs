/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Character Selection UI
 */
using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColorSingleUI : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Image selectedGameObject;
    [SerializeField] private int colorId;
    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerListChanged;
        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerListChanged(object sender, EventArgs e) {
        UpdateIsSelected();
    }

    private void UpdateIsSelected() {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
            selectedGameObject.gameObject.SetActive(true);
        }
        else {
            selectedGameObject.gameObject.SetActive(false);
        }
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerListChanged;
    }
}
