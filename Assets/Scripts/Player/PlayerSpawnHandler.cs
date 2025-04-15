using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
 /*   private void OnEnable()
    {
        // Subscribe to the OnPlayerLoaded event
        SceneLoader.OnPlayerLoaded += OnPlayerLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnPlayerLoaded event to avoid memory leaks
        SceneLoader.OnPlayerLoaded -= OnPlayerLoaded;
    }

    private void OnPlayerLoaded(GameObject player)
    {
        // Find the SpawnPoint object in the scene
        SpawnPoint spawnPoint = FindFirstObjectByType<SpawnPoint>();
        if (spawnPoint != null)
        {
            MoveToSpawnPoint(player, spawnPoint);
        }
        else
        {
            Debug.LogWarning("SpawnPoint object not found in the scene.");
        }
    }

    private void MoveToSpawnPoint(GameObject player, SpawnPoint spawnPoint)
{
    Debug.Log($"Moving player {player.name} to spawn point at {spawnPoint.transform.position}");
    player.transform.position = spawnPoint.transform.position;
    player.transform.rotation = spawnPoint.transform.rotation;
    Debug.Log($"Player moved to position {player.transform.position}");
}*/
}