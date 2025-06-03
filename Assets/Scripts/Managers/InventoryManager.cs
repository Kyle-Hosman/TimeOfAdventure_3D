using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] private InventorySO inventorySO;

    private Dictionary<string, ItemSO> itemMap;

    private void Awake()
    {
        itemMap = CreateItemMap();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded += ItemAdded;
        GameEventsManager.instance.inventoryEvents.onItemRemoved += ItemRemoved;
        GameEventsManager.instance.inventoryEvents.onUseItem += HandleUseItem;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded -= ItemAdded;
        GameEventsManager.instance.inventoryEvents.onItemRemoved -= ItemRemoved;
        GameEventsManager.instance.inventoryEvents.onUseItem -= HandleUseItem;
    }


    private Dictionary<string, ItemSO> CreateItemMap()
    {
        //ItemSO[] allItems = Resources.LoadAll<ItemSO>("Items");
        ItemSO[] allItems = inventorySO.inventoryItems.ToArray();
        Dictionary<string, ItemSO> idToItemMap = new Dictionary<string, ItemSO>();
        foreach (ItemSO item in allItems)
        {
            if (idToItemMap.ContainsKey(item.id))
            {
                //Debug.LogWarning("Duplicate ID found when creating item map: " + item.id);
                continue;
            }
            idToItemMap.Add(item.id, item);
        }
        return idToItemMap;
    }

    public void ItemAdded(ItemSO item)
    {
        if (item != null)
        {
            inventorySO.inventoryItems.Add(item);
        }
        else
        {
            Debug.LogError("Attempted to add a null item.");
        }
    }

    private bool isProcessingItemRemoved = false;

    public void ItemRemoved(ItemSO item)
    {
        if (isProcessingItemRemoved)
        {
            return;
        }

        isProcessingItemRemoved = true;

        try
        {
            if (item != null && inventorySO.inventoryItems.Contains(item))
            {
                inventorySO.inventoryItems.Remove(item);

                // Notify the UI to update the inventory list
                GameEventsManager.instance.inventoryEvents.ItemRemoved(item); // This triggers the event
            }
            else
            {
                Debug.LogError("Attempted to remove an item that is null or not in the inventory.");
            }
        }
        finally
        {
            isProcessingItemRemoved = false;
        }
    }

    private void InvokeItemRemovedEvent(ItemSO item)
    {
        GameEventsManager.instance.inventoryEvents.ItemRemoved(item);
    }

    private void HandleUseItem(ItemSO item)
    {
        if (item != null)
        {
            UseItem(item);
            ItemRemoved(item);

            // Notify the UI to update the inventory list
            GameEventsManager.instance.inventoryEvents.InventoryUpdated();
        }
        else
        {
            Debug.LogError("HandleUseItem: Item is null.");
        }
    }

    public List<ItemSO> GetInventoryItems()
    {
        return new List<ItemSO>(inventorySO.inventoryItems);
    }

    public void UseItem(ItemSO item)
    {
        switch (item.statToChange)
        {
            case ItemSO.StatToChange.Health:
                GameEventsManager.instance.playerEvents.PlayerHealthGained(item.statChangeAmount);
                Debug.Log("Health changed by: " + item.statChangeAmount);
                break;
            case ItemSO.StatToChange.Mana:
                // Implement mana change logic
                break;
            case ItemSO.StatToChange.Stamina:
                // Implement stamina change logic
                break;
            case ItemSO.StatToChange.Strength:
                // Implement strength change logic
                break;
            case ItemSO.StatToChange.Agility:
                // Implement agility change logic
                break;
            case ItemSO.StatToChange.Intelligence:
                // Implement intelligence change logic
                break;
            case ItemSO.StatToChange.Defense:
                // Implement defense change logic
                break;
            default:
                Debug.LogWarning("Unknown stat to change: " + item.statToChange);
                break;
        }
    }
}