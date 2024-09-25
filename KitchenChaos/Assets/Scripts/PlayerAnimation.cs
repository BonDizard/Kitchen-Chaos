/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * player animation
 */
using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    //keep the same name as the animation paramenter used in unity editor
    private const String IS_WALKING = "IsWalking";
    private Animator animator;
    [SerializeField] private Player player;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.getIfThePlayerisWalking());
    }
}
