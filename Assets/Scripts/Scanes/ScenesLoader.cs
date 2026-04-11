using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoader
{
    public static async Task LoadScene(GameState gameState)
    {
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
