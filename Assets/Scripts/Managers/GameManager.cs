using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, IInitializable
{
    public static GameManager Instance { get; private set; }

    public enum GameStateTag
    {
        StartScreen,
        StartGame,
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private GameStateTag startGameSceneName = GameStateTag.StartScreen;
    [SerializeField] private List<GameState> gameStates = new List<GameState>();
    public GameState currentGameState { get; private set; }

    public void Init()
    {
        DebugUtility.WriteInColor($"Initializing game manager...", Color.green);
        gameStates = Resources.LoadAll<GameState>("GameStates/").ToList();
        currentGameState = gameStates.First(state => state.GetTag() == startGameSceneName);
        ScenesLoader.LoadScene(currentGameState);
    }

    public void ChangeGameState(GameStateTag newGameStateTag)
    {
        GameState newGameState = gameStates.First(state => state.GetTag() == newGameStateTag);
        if (newGameState == null)
        {
            Debug.LogError($"Game state with tag '{newGameStateTag}' not found.");
            return;
        }

        DebugUtility.WriteInColor($"Changing game state to '{newGameStateTag}'", Color.green);

        currentGameState = newGameState;
        ScenesLoader.LoadScene(currentGameState);
    }
}
