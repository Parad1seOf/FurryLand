using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemType item;
    [SerializeField] private int amount = 1;

    public void OnTriggerEnter(Collider other)
    {
        InventorySystem inventory = other.GetComponent<InventorySystem>();
        if (inventory == null) return;

        inventory.AddItem(item, amount);
        Destroy(gameObject);
    }
}
