/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Description: Delivery Counter logic
 */
public class DeliveryCounter : BaseCounter {


    public static DeliveryCounter Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                DeliverManager.Instance.DeliveryRecipe(plateKitchenObject);
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
