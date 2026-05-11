using UnityEngine;

public class PickupAction : InteractableAction
{
    [SerializeField] ItemType itemType = ItemType.Wood;
    [SerializeField] int amount = 1;


    public override void Execute(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.Magazine:
                var gun = player.GetComponentInChildren<GunSystem>();
                if (gun == null)
                {
                    Debug.LogWarning("[_PickupAction] El jugador no tiene _GunSystem.");
                    return;
                }
                gun.AddMagazines(amount);
                break;

            default:
                var inventory = player.GetComponent<InventorySystem>();
                if (inventory == null)
                {
                    Debug.LogWarning("[_PickupAction] El jugador no tiene _InventorySystem.");
                    return;
                }
                inventory.AddItem(itemType, amount);
                break;
        }

        Debug.Log($"[_PickupAction] Recogido {amount}x {itemType}");
        Destroy(gameObject);
    }
}
