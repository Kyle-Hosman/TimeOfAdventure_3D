using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string managersSceneName = "Managers";
    [SerializeField] private string uiSceneName = "UI";
    [SerializeField] private string playerSceneName = "Player";
    [SerializeField] private string environmentSceneName = "Environment1"; // Set this dynamically if needed

    // Define events for when the Player scene and all scenes are loaded
    public static event Action<GameObject> OnPlayerLoaded;
    public static event Action OnPlayerSceneLoaded;
    public static event Action OnAllScenesLoaded;

    private void Start()
    {
        StartCoroutine(LoadScenesSequentially());
    }

    private IEnumerator LoadScenesSequentially()
    {
        // Load Managers scene
        yield return LoadSceneAsync(managersSceneName);

        // Load UI scene
        yield return LoadSceneAsync(uiSceneName);

        // Wait a frame to ensure the player scene is fully initialized
        yield return null;
        // Load Player scene
        yield return LoadSceneAsync(playerSceneName);
        NotifyPlayerSceneLoaded(); // Notify that the Player scene is loaded
        NotifyPlayerLoaded();      // Notify that the player has been loaded

        // Finally, load the environment scene
        yield return LoadSceneAsync(environmentSceneName);

        // Notify that all scenes are loaded
        NotifyAllScenesLoaded();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null; // Wait until the scene is fully loaded
            }
        }
    }

    private void NotifyPlayerSceneLoaded()
    {
        Debug.Log("Player scene has been loaded.");
        OnPlayerSceneLoaded?.Invoke();
    }

    private void NotifyPlayerLoaded()
    {
        // Find the player GameObject in the loaded scene
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Debug.Log("Player GameObject found and loaded.");
            OnPlayerLoaded?.Invoke(player);
        }
        else
        {
            Debug.LogWarning("Player GameObject not found in the loaded scene.");
        }
    }

    private void NotifyAllScenesLoaded()
    {
        Debug.Log("All scenes have been loaded.");
        OnAllScenesLoaded?.Invoke();
    }

    public void TogglePlayerCamera(bool isActive)
    {
        var playerCamera = GameObject.FindWithTag("MainCamera");
        if (playerCamera != null)
        {
            playerCamera.SetActive(isActive);
        }
    }

    public void SwitchEnvironmentScene(string currentEnvironmentSceneName, string targetSceneName)
    {
        Debug.Log($"Switching environment scene. Unloading: {currentEnvironmentSceneName}, Loading: {targetSceneName}");
        StartCoroutine(SwitchEnvironmentSceneCoroutine(currentEnvironmentSceneName, targetSceneName));
    }

    private IEnumerator SwitchEnvironmentSceneCoroutine(string currentEnvironmentSceneName, string targetSceneName)
    {
        // Unload the current environment scene
        if (!string.IsNullOrEmpty(currentEnvironmentSceneName) && SceneManager.GetSceneByName(currentEnvironmentSceneName).isLoaded)
        {
            Debug.Log($"Unloading scene: {currentEnvironmentSceneName}");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentEnvironmentSceneName);
            while (!unloadOperation.isDone)
            {
                yield return null; // Wait until the scene is fully unloaded
            }
            Debug.Log($"Scene {currentEnvironmentSceneName} successfully unloaded.");
        }
        else
        {
            Debug.LogWarning($"Scene {currentEnvironmentSceneName} is not loaded or is null.");
        }

        // Load the target scene
        if (!string.IsNullOrEmpty(targetSceneName) && !SceneManager.GetSceneByName(targetSceneName).isLoaded)
        {
            Debug.Log($"Loading scene: {targetSceneName}");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            while (!loadOperation.isDone)
            {
                yield return null; // Wait until the scene is fully loaded
            }
            Debug.Log($"Scene {targetSceneName} successfully loaded.");
        }
        else
        {
            Debug.LogWarning($"Scene {targetSceneName} is already loaded or is null.");
        }
    }
}
