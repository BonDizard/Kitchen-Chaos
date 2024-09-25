/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Logic for trash can
 */
public class TrashCounter : BaseCounter {
    public override void Interact(Player player) {
        //if player has object then destroy it
        if (player.HasKitchenObject()) {
            player.GetKitchenObject().DestroySelf();
        }
    }
}