/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: All Player related code handled here
 */
using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Player : NetworkBehaviour, IKitchenObjectParent {
    public event EventHandler OnPickedSomething;
    public static event EventHandler OnAnyPlayerSpwaned;
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData() {
        OnAnyPlayerSpwaned = null;
    }
    public static Player LocalInstance { get; private set; }
    public event EventHandler<onSelectedCounterChangeEventArgs> onSelectedCounterChange;

    public class onSelectedCounterChangeEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private List<Vector2> spawnPositions;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isWalking;
    private Vector3 lastMoveDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Start() {
        // Subscribe to game input events
        GameInput.Instance.OnInteractionPerformed += GameInput_OnInteractionPerformed;
        GameInput.Instance.OnAlternateInteractionPerformed += GameInput_OnAlternateInteractionPerformed;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

        Debug.Log("[Player Init] Player initialized with color: " + playerData.colorId);
    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
            Debug.Log("[Network Spawn] Player is the owner.");
        }

        transform.position = spawnPositions[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        Debug.Log("[Network Spawn] Player spawned at position: " + transform.position);
        OnAnyPlayerSpwaned?.Invoke(this, EventArgs.Empty);

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        Debug.Log("[Client Disconnect] Client disconnected: " + clientId);
        if (clientId == OwnerClientId && HasKitchenObject()) {
            Debug.Log("[Client Disconnect] Destroying kitchen object held by the player.");
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnAlternateInteractionPerformed(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsGamePlaying()) {
            Debug.Log("[Alt Interaction] Alternate interaction performed.");
            if (selectedCounter != null) {
                selectedCounter.InteractAlternate(this);
                Debug.Log("[Alt Interaction] Interacted alternately with counter: " + selectedCounter.name);
            }
        }
    }

    private void GameInput_OnInteractionPerformed(object sender, EventArgs e) {
        if (KitchenGameManger.Instance.IsGamePlaying()) {
            Debug.Log("[Interaction] Interaction performed.");
            if (selectedCounter != null) {
                selectedCounter.Interact(this);
                Debug.Log("[Interaction] Interacted with counter: " + selectedCounter.name);
            }
        }
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }
        HandleThePlayerMovement();
        HandleThePlayerInteraction();
    }

    public bool getIfThePlayerisWalking() {
        return isWalking;
    }

    private void HandleThePlayerInteraction() {
        Vector2 inputPlayerCoordinates = GameInput.Instance.GetPlayerMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinates.x, 0.0f, inputPlayerCoordinates.y).normalized;

        float interactionDistance = 2f;
        if (moveDirection != Vector3.zero) {
            lastMoveDirection = moveDirection;
        }

        Debug.Log("[Raycast] Raycasting in direction: " + lastMoveDirection + " from position: " + transform.position);

        if (Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactionDistance, counterLayerMask)) {
            Debug.Log("[Raycast] Raycast hit: " + raycastHit.transform.name + " at distance: " + raycastHit.distance);
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                    Debug.Log("[Raycast] New counter selected: " + baseCounter.name);
                }
            }
            else {
                SetSelectedCounter(null);
                Debug.Log("[Raycast] Hit object is not a counter.");
            }
        }
        else {
            SetSelectedCounter(null);
            Debug.Log("[Raycast] No counter detected.");
        }
    }

    private void HandleThePlayerMovement() {
        Vector2 inputPlayerCoordinatesVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinatesVector.x, 0.0f, inputPlayerCoordinatesVector.y).normalized;

        float moveDistance = Time.deltaTime * movementSpeed;
        float playerRadius = 0.6f;

        Debug.Log("[Movement] Moving player with direction: " + moveDirection + " and distance: " + moveDistance);

        // Checking for collisions using BoxCast
        bool canThePlayerMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionLayerMask);

        if (!canThePlayerMove) {
            Debug.Log("[Collision] Detected collision in direction: " + moveDirection);

            // Attempting to move on the X axis if a collision is detected
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canThePlayerMove = (moveDirection.x < -0.5f || moveDirection.x > 0.5f) &&
                                !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, moveDistance, collisionLayerMask);

            if (canThePlayerMove) {
                moveDirection = moveDirectionX;
                Debug.Log("[Movement] Adjusted movement to X axis.");
            }
            else {
                // Attempting to move on the Z axis if X movement fails
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canThePlayerMove = (moveDirection.z < -0.5f || moveDirection.z > 0.5f) &&
                                    !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionZ, Quaternion.identity, moveDistance, collisionLayerMask);

                if (canThePlayerMove) {
                    moveDirection = moveDirectionZ;
                    Debug.Log("[Movement] Adjusted movement to Z axis.");
                }
                else {
                    Debug.Log("[Collision] Collision in both X and Z directions, player cannot move.");
                }
            }
        }

        if (canThePlayerMove) {
            transform.position += moveDirection * moveDistance;
            Debug.Log("[Movement] Player moved to position: " + transform.position);
        }

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

        isWalking = moveDirection != Vector3.zero;
        Debug.Log("[Movement] Player walking status: " + isWalking);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        Debug.Log("[Selection] Selected counter changed: " + (selectedCounter != null ? selectedCounter.name : "None"));
        onSelectedCounterChange?.Invoke(this, new onSelectedCounterChangeEventArgs {
            selectedCounter = selectedCounter,
        });
    }

    public Transform GetKitchenObjectFollowTrasform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
            Debug.Log("[KitchenObject] Kitchen object picked up: " + kitchenObject.name);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
        Debug.Log("[KitchenObject] Kitchen object cleared.");
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
