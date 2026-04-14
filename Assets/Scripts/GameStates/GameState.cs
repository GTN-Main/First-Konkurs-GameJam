using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameState", menuName = "My/GameState")]
public class GameState : ScriptableObject
{
    [SerializeField]
    private GameManager.GameStateTag gameStateTag;

    public GameManager.GameStateTag GetTag() => gameStateTag;

#if UNITY_EDITOR
    [Header("None declarates that on this state start the scene is not changeing")]
    [SerializeField]
    private SceneAsset sceneAsset;
#endif

    [SerializeField, HideInInspector]
    private string sceneName = "";

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
#endif
    }

    public string GetScene()
    {
        return sceneName;
    }

    public string GetSceneName() => sceneName;
}
