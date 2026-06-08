using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField]
    FixedJoystick player1Joystick;

    public FixedJoystick Player1Joystick => player1Joystick;

    [SerializeField]
    FixedJoystick player2Joystick;

    [SerializeField]
    private GameObject interfaceButtons;

    [SerializeField]
    private ButtonPresser interactButtonPlayer1;

    [SerializeField]
    private ButtonPresser interactButtonPlayer2;

    [SerializeField]
    private ButtonPresser attackButtonPlayer1;

    [SerializeField]
    private ButtonPresser attackButtonPlayer2;

    public Vector2 GetJoystickPosition(PlayerTag playerTag)
    {
        switch (playerTag)
        {
            case PlayerTag.Player1:
                return player1Joystick != null ? player1Joystick.Direction : Vector2.zero;
            case PlayerTag.Player2:
                return player2Joystick != null ? player2Joystick.Direction : Vector2.zero;
        }
        return Vector2.zero;
    }

    public bool JoystickHasValue(PlayerTag playerTag)
    {
        switch (playerTag)
        {
            case PlayerTag.Player1:
                return player1Joystick != null && player1Joystick.Direction.magnitude > 0.1f;
            case PlayerTag.Player2:
                return player2Joystick != null && player2Joystick.Direction.magnitude > 0.1f;
        }
        return false;
    }

    public bool GetInteractButtonState(PlayerTag playerTag)
    {
        switch (playerTag)
        {
            case PlayerTag.Player1:
                return interactButtonPlayer1.IsPressed();
            case PlayerTag.Player2:
                return interactButtonPlayer2.IsPressed();
        }
        return false;
    }

    public bool GetAttackButtonState(PlayerTag playerTag)
    {
        switch (playerTag)
        {
            case PlayerTag.Player1:
                return attackButtonPlayer1.IsPressed();
            case PlayerTag.Player2:
                return attackButtonPlayer2.IsPressed();
        }
        return false;
    }

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
        if (player1Joystick == null)
            Debug.LogWarning("Player 1 Joystick reference is missing!");
        if (player2Joystick == null)
            Debug.LogWarning("Player 2 Joystick reference is missing!");
        if (interfaceButtons == null)
            Debug.LogWarning("Interface buttons reference is missing!");

        if (player1Joystick != null)
            player1Joystick.transform.parent.gameObject.SetActive(false);

        if (player2Joystick != null)
            player2Joystick.transform.parent.gameObject.SetActive(false);

        if (interfaceButtons != null)
            interfaceButtons.SetActive(false);
    }

    private void OnEnable()
    {
        globalInputAsset.Enable();
        player1InputAsset.Enable();
        player2InputAsset.Enable();
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        globalInputAsset.Disable();
        player1InputAsset.Disable();
        player2InputAsset.Disable();
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state.GetTag() == GameManager.GameStateTag.StartGame)
        {
            player1Joystick.transform.parent.gameObject.SetActive(true);
            player2Joystick.transform.parent.gameObject.SetActive(true);
            interfaceButtons.SetActive(true);
        }
        else
        {
            player1Joystick.transform.parent.gameObject.SetActive(false);
            player2Joystick.transform.parent.gameObject.SetActive(false);
            interfaceButtons.SetActive(false);
        }
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
