/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: clear counter (placing and picking objects)
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    //player is holding a plate
                    //add the ingrident to the plate and then delete in the clear counter
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else {
                    //player dont have plate something else he carring
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        //counter has a plate
                        if (plateKitchenObject.TryAddIngredients(player.GetKitchenObject().GetKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }

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
