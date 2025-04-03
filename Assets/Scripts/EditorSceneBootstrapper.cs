using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EditorSceneBootstrapper : MonoBehaviour
{
    [Header("Required Scenes")]
    [SerializeField] private string managersSceneName = "Managers";
    [SerializeField] private string uiSceneName = "UI";
    [SerializeField] private string playerSceneName = "Player";

    private void Awake()
    {
        // Check if the game is running in the editor and not from the Managers scene
        if (!Application.isEditor || SceneManager.GetSceneByName(managersSceneName).isLoaded)
        {
            return;
        }

        Debug.Log("EditorSceneBootstrapper: Ensuring required scenes are loaded for testing.");
        StartCoroutine(LoadRequiredScenes());
    }

    private IEnumerator LoadRequiredScenes()
    {
        // Load Managers scene
        if (!SceneManager.GetSceneByName(managersSceneName).isLoaded)
        {
            Debug.Log($"Loading Managers scene: {managersSceneName}");
            yield return SceneManager.LoadSceneAsync(managersSceneName, LoadSceneMode.Additive);
        }

        // Load UI scene
        if (!SceneManager.GetSceneByName(uiSceneName).isLoaded)
        {
            Debug.Log($"Loading UI scene: {uiSceneName}");
            yield return SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);
        }

        // Load Player scene
        if (!SceneManager.GetSceneByName(playerSceneName).isLoaded)
        {
            Debug.Log($"Loading Player scene: {playerSceneName}");
            yield return SceneManager.LoadSceneAsync(playerSceneName, LoadSceneMode.Additive);
        }

        Debug.Log("EditorSceneBootstrapper: All required scenes are loaded.");
    }
}
