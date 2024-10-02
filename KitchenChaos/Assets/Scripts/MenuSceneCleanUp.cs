/*
 * Author: Bharath Kumar S
 * Date: 2-10-2024
 * Description: Cleaning up undestroyed Instances at logic
 */
using Unity.Netcode;
using UnityEngine;

public class MenuSceneCleanUp : MonoBehaviour {
    private void Awake() {
        if (KitchenGameMultiplayer.Instance != null) {
            Destroy(KitchenGameMultiplayer.Instance.gameObject);
        }
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (KitchenGameLobby.Instance != null) {
            Destroy(KitchenGameLobby.Instance.gameObject);
        }
    }
}
