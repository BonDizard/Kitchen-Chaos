using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerOpenCloseAnimation : MonoBehaviour {
    private Animator animator;
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
