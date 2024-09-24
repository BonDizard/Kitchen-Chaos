using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
    [SerializeField] Image barImage;
    [SerializeField] GameObject hasProgressGameObject;
    private IProgressBar hasProgress;
    private void Start() {
        hasProgress = hasProgressGameObject.GetComponent<IProgressBar>();
        if (hasProgress == null) {
            Debug.LogError("Game Object " + hasProgressGameObject + " Does not have the IProgressBar");
        }
        hasProgress.OnProgressChanged += IProgressBar_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void IProgressBar_OnProgressChanged(object sender, IProgressBar.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized == 0 || e.progressNormalized == 1) {
            Hide();
        }
        else {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}