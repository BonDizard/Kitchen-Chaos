using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    private PlayerInputActions playerInputActions;

    public event EventHandler OnInteractionPerformed;

    public event EventHandler OnAlternateInteractionPerformed;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //WhenEver the Interact key is pressed it is lisntened and Interact_Performed is sent
        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.Alternate.performed += AlternateInteract_Performed;
    }
    private void AlternateInteract_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnAlternateInteractionPerformed?.Invoke(this, EventArgs.Empty);
    }
    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractionPerformed?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetPlayerMovementVectorNormailzed() {
        //new input system 
        Vector2 inputPlayerCoordinatesVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        //old input
        // if (Input.GetKey(KeyCode.W)) {
        //     inputPlayerCoordinatesVector.y += 1;
        // }
        // if (Input.GetKey(KeyCode.A)) {
        //     inputPlayerCoordinatesVector.x -= 1;
        // }
        // if (Input.GetKey(KeyCode.S)) {
        //     inputPlayerCoordinatesVector.y -= 1;
        // }
        // if (Input.GetKey(KeyCode.D)) {
        //     inputPlayerCoordinatesVector.x += 1;
        // }

        //for diagonal it will more faster so normailze it when pressing 2 keys simultaneously
        return inputPlayerCoordinatesVector.normalized;

    }
}
