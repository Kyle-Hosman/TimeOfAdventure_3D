using UnityEngine;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void Start()
    {
        // Find the SpawnPoint object in the scene
        SpawnPoint spawnPoint = FindFirstObjectByType<SpawnPoint>();
        if (spawnPoint != null)
        {
            MoveToSpawnPoint(spawnPoint);
        }
        else
        {
            Debug.LogWarning("SpawnPoint object not found in the scene.");
        }
    }

    private void MoveToSpawnPoint(SpawnPoint spawnPoint)
    {
        Debug.Log($"Moving player to spawn point at {spawnPoint.transform.position}");
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
    }
}