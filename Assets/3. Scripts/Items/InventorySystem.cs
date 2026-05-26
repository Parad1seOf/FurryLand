using UnityEngine;
using System;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    private readonly Dictionary<ItemType, int> items = new();

    public event Action<ItemType, int> OnItemChanged;

    public void AddItem(ItemType type, int amount = 1)
    {
        items.TryGetValue(type, out int current);
        items[type] = current + amount;
        OnItemChanged?.Invoke(type, items[type]);

        if (type == ItemType.Wood && ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Wood_Pickup");
    }

    public bool ConsumeItem(ItemType type, int amount = 1)
    {
        if (!HasItem(type, amount)) return false;

        items[type] -= amount;
        if (items[type] <= 0) items.Remove(type);
        OnItemChanged?.Invoke(type, GetCount(type));
        return true;
    }

    public bool HasItem(ItemType type, int amount = 0)
    {
        return items.TryGetValue(type, out int current) && current >= amount;
    }

    public int GetCount(ItemType type)
    {
        return items.TryGetValue(type, out int current) ? current : 0;
    }
}