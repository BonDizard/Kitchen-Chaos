using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter, IKitchenObjectParent {
    [SerializeField] private KitchenObjectsSO kitchenScriptableObject;
    public EventHandler OnPlayerGrabedAnObject;
    public override void Interact(PlayerScript player) {
        if (!player.HasKitchenObject()) {
            //player dont have anything
            KitchenObject.SpawnKitchenObect(kitchenScriptableObject, player);
            OnPlayerGrabedAnObject?.Invoke(this, EventArgs.Empty);
        }
        else {
            //player have an object
        }
    }

}
