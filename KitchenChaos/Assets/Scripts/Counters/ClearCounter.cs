/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * clear counter (placing and picking objects)
 */
public class ClearCounter : BaseCounter {
    public override void Interact(Player player) {
        //if there is onject player pickup
        if (HasKitchenObject()) {
            //the counter has some object
            if (!player.HasKitchenObject()) {
                //the player not have anything
                GetKitchenObject().SetKitchenObjectToParent(player);
            }
            else {
                //the player is having something
            }
        }
        else {
            //the counter is empty
            if (player.HasKitchenObject()) {
                //player has an object
                player.GetKitchenObject().SetKitchenObjectToParent(this);
            }
            else {
                //the player is not having an object
            }
        }
    }
}
