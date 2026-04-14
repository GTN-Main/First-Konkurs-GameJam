using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Settings")]
    [SerializeField]
    private InputActionAsset globalInputAsset;

    [SerializeField]
    private string globalInputAsset_defaultMap = "UI";
    public InputActionMap CurrentMap_global { get; private set; }

    [SerializeField]
    private InputActionAsset player1InputAsset;

    [SerializeField]
    private string player1InputAsset_defaultMap = "Player";
    public InputActionMap CurrentMap_player1 { get; private set; }

    [SerializeField]
    private InputActionAsset player2InputAsset;

    [SerializeField]
    private string player2InputAsset_defaultMap = "Player";
    public InputActionMap CurrentMap_player2 { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Default map initialization
        CurrentMap_global = globalInputAsset.FindActionMap(globalInputAsset_defaultMap);
        CurrentMap_player1 = player1InputAsset.FindActionMap(player1InputAsset_defaultMap);
        CurrentMap_player2 = player2InputAsset.FindActionMap(player2InputAsset_defaultMap);
    }

    private void OnEnable()
    {
        globalInputAsset.Enable();
        player1InputAsset.Enable();
        player2InputAsset.Enable();
    }

    private void OnDisable()
    {
        globalInputAsset.Disable();
        player1InputAsset.Disable();
        player2InputAsset.Disable();
    }

    /// <summary>
    /// Switches the current global input action map to the one specified by mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void SwitchGlobalActionMap(string mapName)
    {
        globalInputAsset.Disable();
        CurrentMap_global = globalInputAsset.FindActionMap(mapName);
        globalInputAsset.Enable();
    }

    /// <summary>
    /// Switches the current player1 input action map to the one specified by mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void SwitchPlayer1ActionMap(string mapName)
    {
        player1InputAsset.Disable();
        CurrentMap_player1 = player1InputAsset.FindActionMap(mapName);
        player1InputAsset.Enable();
    }

    /// <summary>
    /// Switches the current player2 input action map to the one specified by mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void SwitchPlayer2ActionMap(string mapName)
    {
        player2InputAsset.Disable();
        CurrentMap_player2 = player2InputAsset.FindActionMap(mapName);
        player2InputAsset.Enable();
    }
}
