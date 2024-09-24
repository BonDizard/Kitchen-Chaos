using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    private const String IS_WALKING = "IsWalking";
    private Animator animator;
    [SerializeField] private PlayerScript player;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.getIfThePlayerisWalking());
    }
}
