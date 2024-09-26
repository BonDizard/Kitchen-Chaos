/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: All Player related code handeled here
 */

using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    public event EventHandler OnPickedSomething;
    public static Player Instance { get; private set; }
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



    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is more than one Instance of Player");
        }
        Instance = this;
    }
    private void Start() {
        //subsribe to the events in gameinput and listen to them
        gameInput.OnInteractionPerformed += GameInput_OnInteractionPerformed;
        gameInput.OnAlternateInteractionPerformed += GameInput_OnAlternateInteractionPerformed;
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
        HandleThePlayerMovement();
        HandleThePlayerIntercation();
    }
    public bool getIfThePlayerisWalking() {
        return isWalking;
    }
    private void HandleThePlayerIntercation() {
        Vector2 inputPlayerCoordinates = gameInput.GetPlayerMovementVectorNormalized();
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
        Vector2 inputPlayerCoordinatesVector = gameInput.GetPlayerMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputPlayerCoordinatesVector.x, 0.0f, inputPlayerCoordinatesVector.y);

        float playerRadius = 0.7f;
        float PlayerHeight = 2f;

        float distanceToMove = Time.deltaTime * movementSpeed;

        //raycast and check if anything ahead
        bool canThePlayerMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, playerRadius, moveDirection, distanceToMove);
        //is anything there
        if (!canThePlayerMove) {
            //cannot move towards movementDirection
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0);
            //now if player is trying to move diagonal while the object there in front 
            //check if moveDirection contains change in x and there is nothing on left of the player 
            canThePlayerMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, playerRadius, moveDirectionX, distanceToMove);
            //if then move towards
            if (canThePlayerMove) {
                moveDirection = moveDirectionX;
            }
            //else check for z
            else {
                //check if moveDirection contains change in z and there is nothing on left of the player 
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
}
