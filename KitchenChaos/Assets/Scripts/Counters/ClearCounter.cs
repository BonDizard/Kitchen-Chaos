using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClearCounter : BaseCounter {
    public override void Interact(PlayerScript player) {
        if (HasKitchenObject()) {
            //the counter has some object
            if (!player.HasKitchenObject()) {
                //the player not have anything
                GetKitchenObject().SetClearIKitchenObjectParent(player);
            }
            else {
                //the player is having something
            }
        }
        else {
            //the counter is empty
            if (player.HasKitchenObject()) {
                //player has an object
                player.GetKitchenObject().SetClearIKitchenObjectParent(this);
            }
            else {
                //the player is not having an object
            }
        }
    }
}
