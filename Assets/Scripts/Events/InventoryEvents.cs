using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryEvents
{
    public event Action<ItemSO> onItemAdded;
    public event Action<ItemSO> onItemRemoved;
    public event Action<int> onUpdateSelectedSlot;
    public event Action onInventoryUpdated;
    public event Action<ItemSO> onUseItem;

    public void ItemAdded(ItemSO item)
    {
        onItemAdded(item);
    }

    public void ItemRemoved(ItemSO item)
    {
        onItemRemoved(item);
    }

    public void UpdateSelectedSlot(int slotIndex)
    {
        onUpdateSelectedSlot(slotIndex);
    }

    public void InventoryUpdated()
    {
        onInventoryUpdated();
    }

    public void UseItem(ItemSO item)
    {
        onUseItem(item);
    }
}
