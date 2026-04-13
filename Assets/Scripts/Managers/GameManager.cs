using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour, IInitializable
{
    public static GameManager Instance { get; private set; }

    public enum GameStateTag
    {
        StartScreen,
        StartGame,
        EndGame
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

    public async Task Init()
    {
        DebugUtility.WriteInColor($"Initializing game manager...", Color.green);
        gameStates = Resources.LoadAll<GameState>("GameStates/").ToList();
        currentGameState = gameStates.First(state => state.GetTag() == startGameSceneName);
        ScenesLoader.LoadScene(currentGameState);
    }

    public async Task ChangeGameState(GameStateTag newGameStateTag)
    {
        GameState newGameState = gameStates.First(state => state.GetTag() == newGameStateTag);
        if (newGameState == null)
        {
            Debug.LogError($"Game state with tag '{newGameStateTag}' not found.");
            return;
        }

        DebugUtility.WriteInColor($"Changing game state to '{newGameStateTag}'", Color.green);

        currentGameState = newGameState;
        await ScenesLoader.LoadScene(currentGameState);
        OnGameStateChanged?.Invoke(currentGameState);
    }

    public GameStateTag GetCurrentGameStateTag() => currentGameState?.GetTag() ?? GameStateTag.StartScreen;

    public void StartGame()
    {
        DebugUtility.WriteInColor($"Starting game...", Color.green);
        ChangeGameState(GameStateTag.StartGame);
    }

    public void EndGame()
    {
        DebugUtility.WriteInColor($"Ending game...", Color.red);
        ChangeGameState(GameStateTag.EndGame);
    }

    public static bool CanPlayersReturnToEndArea() => WaterContainerManager.Instance.IsWaterLevelFull();

    public event System.Action<GameState> OnGameStateChanged;
}
