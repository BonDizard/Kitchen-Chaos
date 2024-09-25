/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Container animation
 */
using UnityEngine;

public class ContainerOpenCloseAnimation : MonoBehaviour {
    private Animator animator;
    //keep the same name as the animation paramenter used in unity editor
    private const string OPEN_CLOSE = "OpenClose";
    [SerializeField] private ContainerCounter containerCounter;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        containerCounter.OnPlayerGrabedAnObject += ContainerCounter_OnPlayerGrabedAnObject;
    }

    private void ContainerCounter_OnPlayerGrabedAnObject(object sender, System.EventArgs e) {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
