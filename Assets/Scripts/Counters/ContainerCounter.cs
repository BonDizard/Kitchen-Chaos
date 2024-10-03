/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Container logic (spawining ingridents)
 */
using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent {
    [SerializeField] private KitchenObjectsSO kitchenScriptableObject;
    public EventHandler OnPlayerGrabedAnObject;
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            //player dont have anything
            KitchenObject.SpawnKitchenObect(kitchenScriptableObject, player);
            InteractLogicServerRpc();
        }
        else {
            //player have an object
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabedAnObject?.Invoke(this, EventArgs.Empty);
    }

}
