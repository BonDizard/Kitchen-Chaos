/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Delivery Stive sound handed here.
 */
using UnityEngine;

public class CookingCounterSound : MonoBehaviour {
    [SerializeField] private CookingCounter cookingCounter;
    private AudioSource audioSource;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        cookingCounter.OnStateChange += CookingCounter_OnStateChanged;
    }
    private void CookingCounter_OnStateChanged(object sender, CookingCounter.OnStateChangeEventArgs e) {
        bool playSound = e.state == CookingCounter.State.Frying || e.state == CookingCounter.State.Fried;
        if (playSound) {
            audioSource.Play();
        }
        else {
            audioSource.Pause();
        }
    }
}
