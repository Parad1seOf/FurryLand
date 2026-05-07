using System;
using UnityEngine;

public class ResourceCondition : Condition
{
    [SerializeField] private ItemType requiredResource;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool consumesItem = true;


    public override bool MeetsCondition(PlayerController player)
    {
        InventorySystem inventory = player.GetInventory();
        if (!inventory.HasItem(requiredResource, amount)) return false;
        
        return true;
    }

    public override void FulfillCondition(PlayerController player)
    {
        InventorySystem inventory = player.GetInventory();
        if (consumesItem) inventory.ConsumeItem(requiredResource, amount);
    }
}
