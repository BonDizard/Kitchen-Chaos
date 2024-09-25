/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Container logic (spawining ingridents)
 */
using System;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent {
    [SerializeField] private KitchenObjectsSO kitchenScriptableObject;
    public EventHandler OnPlayerGrabedAnObject;
    public override void Interact(Player player) {
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
