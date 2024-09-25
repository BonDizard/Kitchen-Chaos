/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * All inputs of the game are handled here.
 */

using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
    private PlayerInputActions playerInputActions;
    //publish the event
    public event EventHandler OnInteractionPerformed;
    public event EventHandler OnAlternateInteractionPerformed;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Whenever the Interact key is pressed, it listens and triggers the Interact_Performed.
        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.Alternate.performed += AlternateInteract_Performed;
    }

    private void AlternateInteract_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        //if no one subscribed then it will be null so check null before Invoke by ?(null condinal operator)
        OnAlternateInteractionPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractionPerformed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetPlayerMovementVectorNormalized() {
        // New input system
        Vector2 inputPlayerCoordinatesVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Old input (Unity legacy), good for prototype but not recommended for final build.
        //---------------------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------------------

        // Normalize the vector when pressing two keys simultaneously for diagonal movement.
        // This ensures the movement speed remains consistent (magnitude is normalized to 1).
        return inputPlayerCoordinatesVector.normalized;
    }
}
