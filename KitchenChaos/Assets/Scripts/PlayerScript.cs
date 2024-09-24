using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour, IKitchenObjectParent {

    public static PlayerScript Instance { get; private set; }
    public event EventHandler<onSelectedCounterChangeEventArgs> onSelectedCounterChange;

    public class onSelectedCounterChangeEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;


    private bool isWalking;
    private Vector3 lastMoveDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Update() {
        HandleThePlayerMovement();
        HandleThePlayerIntercation();
    }
    private void Start() {
        gameInput.OnInteractionPerformed += GameInput_OnInteractionPerformed;
        gameInput.OnAlternateInteractionPerformed += GameInput_OnAlternateInteractionPerformed;
    }
    private void GameInput_OnAlternateInteractionPerformed(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }
    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is more than one Instance of Player");
        }
        Instance = this;
    }
    private void GameInput_OnInteractionPerformed(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }
    public bool getIfThePlayerisWalking() {
        return isWalking;
    }
    private void HandleThePlayerIntercation() {
        Vector2 inputPlayerCoordinates = gameInput.GetPlayerMovementVectorNormailzed();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinates.x, 0.0f, inputPlayerCoordinates.y);

        float interactonDistance = 2f;
        if (moveDirection != Vector3.zero) {
            lastMoveDirection = moveDirection;
        }
        if (Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactonDistance, counterLayerMask)) {

            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }

            }
            else {
                SetSelectedCounter(null);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }
    private void HandleThePlayerMovement() {
        Vector2 inputPlayerCoordinatesVector = gameInput.GetPlayerMovementVectorNormailzed();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinatesVector.x, 0.0f, inputPlayerCoordinatesVector.y);

        float playerRadius = 0.7f;
        float PlayerHeight = 2f;

        float distanceToMove = Time.deltaTime * movementSpeed;
        bool canThePlayerMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, playerRadius, moveDirection, distanceToMove);



        if (!canThePlayerMove) {
            //cannot move towards movementDirectionForThePlayer
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0);
            canThePlayerMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, playerRadius, moveDirectionX, distanceToMove);

            if (canThePlayerMove) {
                moveDirection = moveDirectionX;
            }
            else {
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z);
                canThePlayerMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, playerRadius, moveDirectionZ, distanceToMove);

                if (canThePlayerMove) {
                    moveDirection = moveDirectionZ;

                }
                else {
                    //do nothing
                }
            }
        }
        if (canThePlayerMove) {
            //transform.position need a vector3 not vector2 so movementDirectionForThePlayer
            transform.position += moveDirection * distanceToMove;
        }
        //to make the player turn towards the move direction forward is used
        //Slerp is used to make smooth turns
        float rotationSpeed = 10f;

        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

        isWalking = moveDirection != Vector3.zero;
    }
    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        onSelectedCounterChange?.Invoke(this, new onSelectedCounterChangeEventArgs {
            selectedCounter = selectedCounter,
        });
    }

    public Transform GetKitchenObjectFollowTrasform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
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
}
