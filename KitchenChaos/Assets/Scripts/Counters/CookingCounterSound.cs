/*
 * Author: Bharath Kumar S
 * Date: 26-09-2024
 * Delivery Stive sound handed here.
 */
using UnityEngine;

public class CookingCounterSound : MonoBehaviour {
    [SerializeField] private CookingCounter cookingCounter;
    private AudioSource audioSource;
    private float warningSoundTimer;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
    private bool playWarningSound;
    private void Start() {
        cookingCounter.OnStateChange += CookingCounter_OnStateChanged;
        cookingCounter.OnProgressChanged += CookingCounter_OnProgressChanged;
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
    private void CookingCounter_OnProgressChanged(object sender, IProgressBar.OnProgressChangedEventArgs e) {
        float burnShowProgress = 0.5f;
        playWarningSound = cookingCounter.IsFried() && e.progressNormalized >= burnShowProgress;
    }
    private void Update() {
        if (playWarningSound) {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f) {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayBuringWarningSound(gameObject.transform.position);
            }
        }
    }
}
