using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO item;
    private CircleCollider2D circleCollider;
    private SpriteRenderer visual;

    private void Awake() 
    {
        circleCollider = GetComponent<CircleCollider2D>();
        visual = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
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

                //Debug.Log("Calling ItemAdded event for item: " + item.itemName);
                GameEventsManager.instance.inventoryEvents.ItemAdded(item);
                //Debug.Log("Picked up item: " + item.itemName);

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Trigger entered by non-player object.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Exception in OnTriggerEnter2D: " + ex.Message);
            Debug.LogError("Stack Trace: " + ex.StackTrace);
        }
    }
}
