using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string playSceneName; // The name of the scene to load when Play is clicked

    // This method will be called when the Play button is clicked
    public void OnPlayButtonClicked()
    {
        if (!string.IsNullOrEmpty(playSceneName))
        {
            Debug.Log($"Loading scene: {playSceneName}");
            SceneManager.LoadScene(playSceneName);
        }
        else
        {
            Debug.LogWarning("Play scene name is not set in the inspector!");
        }
    }

    // Optional: Add a method to quit the game
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
