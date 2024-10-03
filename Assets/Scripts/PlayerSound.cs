/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Player walking sound handled here.
 */
using UnityEngine;

public class PlayerSound : MonoBehaviour {
    private Player player;

    float footStepTimer;
    float footStepTimerMax = .1f;
    private void Awake() {
        player = GetComponent<Player>();
    }
    private void Update() {
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0f) {
            footStepTimer = footStepTimerMax;
            if (player.getIfThePlayerisWalking()) {
                float volume = 1f;
                SoundManager.Instance.PlayWalkingSound(player.transform.position, volume);
            }
        }
    }
}
