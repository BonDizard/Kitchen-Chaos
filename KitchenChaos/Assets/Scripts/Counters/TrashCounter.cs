/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Logic for trash can
 */
using System;

public class TrashCounter : BaseCounter {
    public static event EventHandler OnAnyObjectTrashed;
    new public static void ResetStaticData() {
        OnAnyObjectTrashed = null;
    }
    public override void Interact(Player player) {
        //if player has object then destroy it
        if (player.HasKitchenObject()) {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}