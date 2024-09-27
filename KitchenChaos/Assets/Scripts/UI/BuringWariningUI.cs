/*
 * Author: Bharath Kumar S
 * Date: 27-09-2024
 * Description: Burning meat UI 
 */
using UnityEngine;

public class BuringWariningUI : MonoBehaviour {
    [SerializeField] private CookingCounter cookingCounter;
    private void Start() {
        cookingCounter.OnProgressChanged += CookingCounter_OnProgressChanged;
        Hide();
    }

    private void CookingCounter_OnProgressChanged(object sender, IProgressBar.OnProgressChangedEventArgs e) {
        float burnShowProgress = 0.5f;
        bool show = cookingCounter.IsFried() && e.progressNormalized >= burnShowProgress;
        if (show) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
