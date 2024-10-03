/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * All inputs of the game are handled here.
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    const string PLAYER_PREFS_INPUT_BINDING = "PlayerInputBinding";
    public enum Binding {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        Alternate_Interact,
        Pause,
    }
    public event EventHandler OnBingingRebind;
    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;
    //publish the event
    public event EventHandler OnInteractionPerformed;
    public event EventHandler OnAlternateInteractionPerformed;
    public event EventHandler OnPauseButtonClicked;

    private void Awake() {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Whenever the Interact key is pressed, it listens and triggers the Interact_Performed.
        playerInputActions.Player.Interact.performed += Interact_Performed;
        playerInputActions.Player.Alternate.performed += AlternateInteract_Performed;
        playerInputActions.Player.Pause.performed += Pause_Performed;
        if (PlayerPrefs.HasKey(PLAYER_PREFS_INPUT_BINDING)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_INPUT_BINDING));
        }
    }
    private void OnDestroy() {
        playerInputActions.Player.Interact.performed -= Interact_Performed;
        playerInputActions.Player.Alternate.performed -= AlternateInteract_Performed;
        playerInputActions.Player.Pause.performed -= Pause_Performed;

        playerInputActions.Dispose();
    }
    private void Pause_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseButtonClicked?.Invoke(this, EventArgs.Empty);
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

    public string GetBindingText(Binding binding) {
        switch (binding) {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Alternate_Interact:
                return playerInputActions.Player.Alternate.bindings[0].ToDisplayString();
        }
    }
    public void RebindBinding(Binding binding, Action onActionRebound) {
        playerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;
        switch (binding) {

            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.Alternate_Interact:
                inputAction = playerInputActions.Player.Alternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }
        inputAction.PerformInteractiveRebinding(bindingIndex)
        .OnComplete((callback) => {
            callback.Dispose();
            playerInputActions.Player.Enable();
            onActionRebound();
            //to save locally the rebinding
            OnBingingRebind?.Invoke(this, EventArgs.Empty);
            PlayerPrefs.SetString(PLAYER_PREFS_INPUT_BINDING, playerInputActions.SaveBindingOverridesAsJson());
        })
        .Start();
    }
}
