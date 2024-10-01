/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: proggress bar l
 */
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
    [SerializeField] Image barImage;
    [SerializeField] GameObject hasProgressGameObject;
    private IHasProgress hasProgress;
    private void Start() {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        if (hasProgress == null) {
            Debug.LogError("Game Object " + hasProgressGameObject + " Does not have the IProgressBar");
        }
        hasProgress.OnProgressChanged += IProgressBar_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void IProgressBar_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;
        //if bar is full or empty hide
        if (e.progressNormalized == 0f || e.progressNormalized == 1f) {
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