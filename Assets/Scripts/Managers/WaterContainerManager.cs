using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WaterContainerManager : MonoBehaviour, IInitializable
{
    public static WaterContainerManager Instance { get; private set; }

    [SerializeField] private GameObject panelWaterContainer;
    [SerializeField] private RectTransform waterLevelIndicator;
    [SerializeField] private Image waterLampFullIndicator;
    [SerializeField] private Color waterLampFullColor = Color.cyan;
    [SerializeField] private Color waterLampEmptyColor = Color.gray;

    [SerializeField] private float waterPercentage = 0f;
    public bool IsWaterLevelFull() => waterPercentage >= 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public async Task Init()
    {
        HideWaterContainerUI();
    }

    void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state.GetTag() == GameManager.GameStateTag.StartGame)
        {
            ShowWaterContainerUI();
            waterPercentage = 0f;
            SetUpWaterLevel(waterPercentage);
            waterLampFullIndicator.color = waterLampEmptyColor;
        }
        else
        {
            HideWaterContainerUI();
        }
    }

    public float GetWaterPercentage()
    {
        return waterPercentage;
    }

    [ContextMenu("Add Water")]
    public void AddWaterRandom()
    {
        AddWater(UnityEngine.Random.Range(0.1f, 0.3f));
        UpdateWaterUI();
    }

    public void AddWater(float amount)
    {
        waterPercentage = Mathf.Clamp01(waterPercentage + amount);
        UpdateWaterUI();
    }

    public void SetWaterPercentage(float percentage)
    {
        waterPercentage = Mathf.Clamp01(percentage);
        UpdateWaterUI();
    }

    [ContextMenu("Update Water UI")]
    private void UpdateWaterUI()
    {
        UpdateWaterLevel(waterPercentage);
        waterLampFullIndicator.color = waterPercentage >= 1f ? waterLampFullColor : waterLampEmptyColor;
    }

    private void ShowWaterContainerUI()
    {
        panelWaterContainer.SetActive(true);
    }

    private void HideWaterContainerUI()
    {
        panelWaterContainer.SetActive(false);
    }

    /// <summary>
    /// Sets up the water level indicator based on the starting percentage.
    /// </summary>
    /// <param name="startPercent">Water height as a percentage (0.0 to 1.0)</param>
    public void SetUpWaterLevel(float startPercent)
    {
        waterLevelIndicator.localScale = Vector3.one;
        waterLevelIndicator.anchorMin = new Vector2(0, 0);
        waterLevelIndicator.anchorMax = new Vector2(1, startPercent);

        waterLevelIndicator.offsetMin = waterLevelIndicator.offsetMax = Vector2.zero;
    }

    public void UpdateWaterLevel(float newEndPercent)
    {

        waterLevelIndicator.anchorMax = new Vector2(1, newEndPercent);
        waterLevelIndicator.offsetMin = new Vector2(0, 0);
        waterLevelIndicator.offsetMax = Vector2.zero;
    }
}
