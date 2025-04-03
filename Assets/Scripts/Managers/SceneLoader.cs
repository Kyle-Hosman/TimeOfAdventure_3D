using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string managersSceneName = "Managers";
    [SerializeField] private string uiSceneName = "UI";
    [SerializeField] private string playerSceneName = "Player";
    [SerializeField] private string environmentSceneName = "Environment1"; // Set this dynamically if needed

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

        // Load Player scene
        yield return LoadSceneAsync(playerSceneName);

        // Finally, load the environment scene
        yield return LoadSceneAsync(environmentSceneName);
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

    public void TogglePlayerCamera(bool isActive)
    {
        var playerCamera = GameObject.FindWithTag("MainCamera");
        if (playerCamera != null)
        {
            playerCamera.SetActive(isActive);
        }
    }

    // Example usage: Call this method when switching between environment-specific and player-centric cameras.

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
