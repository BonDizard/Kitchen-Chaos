/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Delivery Counter logic
 */
using UnityEngine;

public class DeliveryCounter : BaseCounter {
    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                //only accepts plate
                player.GetKitchenObject().DestroySelf();
            }
        }
    }
}
