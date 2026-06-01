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
                    return;
                }
                gun.AddMagazines(amount);
                break;

            default:
                var inventory = player.GetComponent<InventorySystem>();
                if (inventory == null)
                {
                    return;
                }
                inventory.AddItem(itemType, amount);
                break;
        }

        Destroy(gameObject);
    }
}
