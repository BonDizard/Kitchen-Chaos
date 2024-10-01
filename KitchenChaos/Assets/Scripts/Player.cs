/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: All Player related code handeled here
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

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

        transform.position = spawnPositions[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpwaned?.Invoke(this, EventArgs.Empty);
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId && HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }
    private void Start() {
        //subsribe to the events in gameinput and listen to them
        GameInput.Instance.OnInteractionPerformed += GameInput_OnInteractionPerformed;
        GameInput.Instance.OnAlternateInteractionPerformed += GameInput_OnAlternateInteractionPerformed;
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    private void GameInput_OnAlternateInteractionPerformed(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsGamePlaying()) {
            if (selectedCounter != null) {
                selectedCounter.InteractAlternate(this);
            }
        }
    }
    private void GameInput_OnInteractionPerformed(object sender, System.EventArgs e) {
        if (KitchenGameManger.Instance.IsGamePlaying()) {
            if (selectedCounter != null) {
                selectedCounter.Interact(this);
            }
        }
    }
    private void Update() {
        if (!IsOwner) {
            return;
        }
        HandleThePlayerMovement();
        HandleThePlayerIntercation();

    }
    public bool getIfThePlayerisWalking() {
        return isWalking;
    }
    private void HandleThePlayerIntercation() {
        Vector2 inputPlayerCoordinates = GameInput.Instance.GetPlayerMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinates.x, 0.0f, inputPlayerCoordinates.y);

        float interactonDistance = 2f;
        //here last distance is used because when not moving movedir becomes 0 and raycast wont returnt collision
        if (moveDirection != Vector3.zero) {
            lastMoveDirection = moveDirection;
        }
        //layer mask to only interact with perticular object 
        //use the layers in the unity ui 
        if (Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactonDistance, counterLayerMask)) {
            //get the objects transform of which the player is colliding
            //TryGetComponent is same as GetComponent but auto checks nulls and return true if found
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }

            }
            else {
                //if the object in front is not a base counter
                SetSelectedCounter(null);
            }
        }
        else {
            //if nothing is in front 
            SetSelectedCounter(null);
        }
    }
    private void HandleThePlayerMovement() {
        Vector2 inputPlayerCoordinatesVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinatesVector.x, 0.0f, inputPlayerCoordinatesVector.y);

        float playerRadius = 0.7f;
        float distanceToMove = Time.deltaTime * movementSpeed;

        //raycast and check if anything ahead
        bool canThePlayerMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, distanceToMove, collisionLayerMask);
        //is anything there
        if (!canThePlayerMove) {
            //cannot move towards movementDirection
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0);
            //now if player is trying to move diagonal while the object there in front 
            //check if moveDirection contains change in x and there is nothing on left of the player 
            canThePlayerMove = (moveDirection.x < -0.5f || moveDirection.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, distanceToMove, collisionLayerMask);
            //if then move towards
            if (canThePlayerMove) {
                moveDirection = moveDirectionX;
            }
            //else check for z
            else {
                //check if moveDirection contains change in z and there is nothing on left of the player 
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z);
                canThePlayerMove = (moveDirection.z < -0.5f || moveDirection.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionZ, Quaternion.identity, distanceToMove, collisionLayerMask);

                if (canThePlayerMove) {
                    moveDirection = moveDirectionZ;

                }
                else {
                    //do nothing
                }
            }
        }
        if (canThePlayerMove) {
            transform.position += moveDirection * distanceToMove;
        }
        float rotationSpeed = 10f;

        //to make the player turn towards the move direction forward is used
        //Slerp is used to make smooth turns(Interpolate between player turns)
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

        isWalking = moveDirection != Vector3.zero;
    }
    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        //fire the event 
        onSelectedCounterChange?.Invoke(this, new onSelectedCounterChangeEventArgs {
            selectedCounter = selectedCounter,
        });
    }

    public Transform GetKitchenObjectFollowTrasform() {
        return kitchenObjectHoldPoint;
    }

    //called when picked something
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }
    public void ClearKitchenObject() {
        kitchenObject = null;
    }
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
