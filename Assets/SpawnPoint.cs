using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab

    private void Start()
    {
        // Subscribe to the SceneLoader's event when the player is loaded
        SceneLoader.OnPlayerLoaded += OnPlayerLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneLoader.OnPlayerLoaded -= OnPlayerLoaded;
    }

    private void OnPlayerLoaded(GameObject player)
    {
        // Move the player to the spawn point's position and rotation
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
