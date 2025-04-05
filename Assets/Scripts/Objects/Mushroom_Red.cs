using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Mushroom_Red : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float respawnTimeSeconds = 8;
    [SerializeField] private int mushroomsCollected = 1;

    private BoxCollider boxCollider;
    private MeshRenderer visual;

    private void Awake() 
    {
        boxCollider = GetComponent<BoxCollider>();
        visual = GetComponentInChildren<MeshRenderer>();

        // Ensure the BoxCollider is set as a trigger
        boxCollider.isTrigger = true;
    }

    public void CollectMushroom() 
    {
        boxCollider.enabled = false;
        visual.gameObject.SetActive(false);
        GameEventsManager.instance.mushroomEvents.MushroomChange(mushroomsCollected);
        GameEventsManager.instance.miscEvents.MushroomCollected();
        //StopAllCoroutines();
        //StartCoroutine(RespawnAfterTime());
    }

    private IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnTimeSeconds);
        boxCollider.enabled = true;
        visual.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider otherCollider) 
    {
        if (otherCollider.CompareTag("Player"))
        {
            CollectMushroom();
        }
    }
}
