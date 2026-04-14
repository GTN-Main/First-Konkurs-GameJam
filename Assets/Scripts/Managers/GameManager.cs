using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour, IInitializable
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private CameraController cC;

    public Camera GetCamera() => mainCamera;

    public enum GameStateTag
    {
        StartScreen,
        StartGame,
        WonGame,
        LooseGame,
        EndScreenVictory,
        EndScreenDefeat,
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

    [SerializeField]
    private GameStateTag startGameSceneName = GameStateTag.StartScreen;

    [SerializeField]
    private List<GameState> gameStates = new List<GameState>();
    public GameState currentGameState { get; private set; }

    public async Task Init()
    {
        DebugUtility.WriteInColor($"Initializing game manager...", Color.green);
        gameStates = Resources.LoadAll<GameState>("GameStates/").ToList();
        currentGameState = gameStates.First(state => state.GetTag() == startGameSceneName);
        await ScenesLoader.LoadScene(currentGameState);
        CreateCamera();
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

    public GameStateTag GetCurrentGameStateTag() =>
        currentGameState?.GetTag() ?? GameStateTag.StartScreen;

    public async Task StartGame()
    {
        DebugUtility.WriteInColor($"Starting game...", Color.green);
        await ChangeGameState(GameStateTag.StartGame);
        cC.Init();
        cC.enabled = true;
    }

    public async Task WonGame()
    {
        cC.enabled = false;
        DebugUtility.WriteInColor($"Ending game...", Color.red);
        await ChangeGameState(GameStateTag.WonGame);
        MyAudioEffects.Instance.DoEffect("Won", mainCamera.transform.position, 1f);
        await Task.Delay(2000);
        await ChangeGameState(GameStateTag.EndScreenVictory);
        PlayerSpawnManager.Instance.ResetPlayers();
    }

    public async Task LooseGame()
    {
        cC.enabled = false;
        DebugUtility.WriteInColor($"Players lost the game...", Color.red);
        await ChangeGameState(GameStateTag.LooseGame);
        MyAudioEffects.Instance.DoEffect("Loose", mainCamera.transform.position, 1f);
        await Task.Delay(2000);
        await ChangeGameState(GameStateTag.EndScreenDefeat);
        PlayerSpawnManager.Instance.ResetPlayers();
    }

    public void BackToStartScreen()
    {
        DebugUtility.WriteInColor($"Returning to start screen...", Color.green);
        ChangeGameState(GameStateTag.StartScreen);
    }

    public static bool CanPlayersReturnToEndArea() =>
        WaterContainerManager.Instance.IsWaterLevelFull();

    public event System.Action<GameState> OnGameStateChanged;

    public void CreateCamera()
    {
        if (mainCamera != null)
        {
            Debug.LogWarning("Main camera already exists. Skipping camera creation.");
            return;
        }
        GameObject camPrefab = Resources.Load<GameObject>("Prefabs/Setup/MainCamera");
        if (camPrefab == null)
        {
            Debug.LogError(
                "Main camera prefab not found in Resources/Prefabs/Setup/MainCamera. Please ensure it exists."
            );
            return;
        }
        mainCamera = Instantiate(camPrefab).GetComponent<Camera>();
        mainCamera.transform.SetParent(transform);
        cC = mainCamera.GetComponent<CameraController>();
        cC.enabled = false;
    }
}
