using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    // private void Start()
    // {
    //     // Subscribe to the SceneLoader's OnPlayerLoaded event
    //     SceneLoader.OnPlayerLoaded += OnPlayerLoaded;
    // }

    // private void OnDestroy()
    // {
    //     // Unsubscribe from the event to avoid memory leaks
    //     SceneLoader.OnPlayerLoaded -= OnPlayerLoaded;
    // }

    // private void OnPlayerLoaded(GameObject player)
    // {
    //     // Move the player to the spawn point
    //     MovePlayerToSpawnPoint(player);
    // }

    // private void MovePlayerToSpawnPoint(GameObject player)
    // {
    //     Debug.Log($"Moving player to spawn point at {transform.position}");
    //     player.transform.position = transform.position;
    //     player.transform.rotation = transform.rotation;
    // }
}
