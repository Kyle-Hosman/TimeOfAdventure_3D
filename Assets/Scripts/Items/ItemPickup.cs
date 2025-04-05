using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO item;
    private BoxCollider boxCollider;
    private MeshRenderer visual;

    private void Awake() 
    {
        boxCollider = GetComponent<BoxCollider>();
        visual = GetComponentInChildren<MeshRenderer>();

        // Ensure the BoxCollider is set as a trigger
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.CompareTag("Player"))
            {
                if (item == null)
                {
                    Debug.LogError("ItemSO is not set on the ItemPickup script.");
                    return;
                }

                if (GameEventsManager.instance == null)
                {
                    Debug.LogError("GameEventsManager instance is null.");
                    return;
                }

                if (GameEventsManager.instance.inventoryEvents == null)
                {
                    Debug.LogError("GameEventsManager inventoryEvents is null.");
                    return;
                }

                if (string.IsNullOrEmpty(item.itemName))
                {
                    Debug.LogError("ItemSO itemName is null or empty.");
                    return;
                }

                // Add the item to the inventory
                GameEventsManager.instance.inventoryEvents.ItemAdded(item);
                Debug.Log("Picked up item: " + item.itemName);

                // Destroy the item after pickup
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Trigger entered by non-player object.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception in OnTriggerEnter: " + ex.Message);
            Debug.LogError("Stack Trace: " + ex.StackTrace);
        }
    }
}
