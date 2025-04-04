using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab

    private void Start()
    {
        // Subscribe to the SceneLoader's OnAllScenesLoaded event
        SceneLoader.OnAllScenesLoaded += OnAllScenesLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneLoader.OnAllScenesLoaded -= OnAllScenesLoaded;
    }

    private void OnAllScenesLoaded()
    {
        // Move the player to the spawn point after all scenes are loaded
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            MovePlayerToSpawnPoint(player);
        }
        else
        {
            Debug.LogWarning("Player GameObject not found when all scenes were loaded.");
        }
    }

    private void MovePlayerToSpawnPoint(GameObject player)
    {
        Debug.Log($"Moving player to spawn point at {transform.position}");
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
    }

    private void SpawnPlayer()
    {
        // Check if the player already exists in the scene
        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer == null)
        {
            // Instantiate the player at the spawn point's position and rotation
            Instantiate(playerPrefab, transform.position, transform.rotation);
        }
        else
        {
            // Move the existing player to the spawn point
            existingPlayer.transform.position = transform.position;
            existingPlayer.transform.rotation = transform.rotation;
        }
    }
}
