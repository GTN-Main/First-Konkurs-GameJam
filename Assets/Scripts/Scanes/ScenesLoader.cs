using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader
{
    public static async Task LoadScene(GameState gameState)
    {
        if (gameState == null)
        {
            Debug.LogError("GameState is null. Cannot load scene.");
            return;
        }
    
        //  No need to load scene if GameState doesn't specify one
        if (gameState.GetSceneName() == null || gameState.GetSceneName().Trim() == "")
        {
            return;
        }

        string sceneToLoadName = gameState.GetSceneName();
        string currentSceneName = GetCurrentScene().name;
        if (currentSceneName == sceneToLoadName)
        {
            Debug.Log($"Scene '{sceneToLoadName}' is already loaded. No need to load it again.");
            return;
        }

        DebugUtility.WriteInColor($"Loading scene '{sceneToLoadName}'...", Color.cyan);
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneToLoadName);
        await loadingScene;
        DebugUtility.WriteInColor($"Scene '{sceneToLoadName}' loaded successfully.", Color.green);
    }
    
    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
