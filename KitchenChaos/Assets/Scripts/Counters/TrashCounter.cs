/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Logic for trash can
 */
using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter {
    public static event EventHandler OnAnyObjectTrashed;
    new public static void ResetStaticData() {
        OnAnyObjectTrashed = null;
    }
    public override void Interact(Player player) {
        //if player has object then destroy it
        if (player.HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            InteractLogicServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}