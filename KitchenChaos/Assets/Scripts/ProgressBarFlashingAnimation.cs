/*
 * Author: Bharath Kumar S
 * Date: 27-09-2024
 * Description: Progress Bar Flashing Animation
 */
using UnityEngine;

public class ProgressBarFlashingAnimation : MonoBehaviour {
    private string IS_FLASHING = "IsFlashing";
    [SerializeField] private CookingCounter cookingCounter;
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        cookingCounter.OnProgressChanged += CookingCounter_OnProgressChanged;
        animator.SetBool(IS_FLASHING, false);
    }

    private void CookingCounter_OnProgressChanged(object sender, IProgressBar.OnProgressChangedEventArgs e) {
        float burnShowProgress = 0.5f;
        bool show = cookingCounter.IsFried() && e.progressNormalized >= burnShowProgress;
        Debug.LogWarning(show);
        if (show) {
            animator.SetBool(IS_FLASHING, show);
        }
    }
}
