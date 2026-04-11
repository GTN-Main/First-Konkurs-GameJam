using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private string defaultMap = "Player";

    public InputActionMap CurrentMap { get; private set; }

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
        CurrentMap = inputAsset.FindActionMap(defaultMap);
    }

    private void OnEnable() => inputAsset.Enable();
    private void OnDisable() => inputAsset.Disable();

    /// <summary>
    /// Switches the current input action map to the one specified by mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void SwitchActionMap(string mapName)
    {
        inputAsset.Disable();
        CurrentMap = inputAsset.FindActionMap(mapName);
        inputAsset.Enable();
    }
}
