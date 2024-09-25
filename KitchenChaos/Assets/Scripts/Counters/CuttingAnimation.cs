/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Cutting Animation
 */
using UnityEngine;

public class CuttingAnimation : MonoBehaviour {
    private Animator animator;
    private const string CUT = "Cut";
    [SerializeField] private CuttingCounter cuttingCounter;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    private void CuttingCounter_OnCut(object sender, System.EventArgs e) {
        animator.SetTrigger(CUT);
    }
}
