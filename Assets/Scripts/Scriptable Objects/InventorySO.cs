using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/InventorySO", order = 1)]
public class InventorySO : ScriptableObject
{
    public List<ItemSO> inventoryItems = new List<ItemSO>();
}