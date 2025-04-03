using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField] private string targetSceneName; // The scene to load
    [SerializeField] private string currentEnvironmentSceneName; // The current environment scene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered the trigger. Switching from {currentEnvironmentSceneName} to {targetSceneName}.");
            SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
            if (sceneLoader != null)
            {
                sceneLoader.SwitchEnvironmentScene(currentEnvironmentSceneName, targetSceneName);
            }
            else
            {
                Debug.LogError("SceneLoader not found in the Managers scene.");
            }
        }
    }
}
