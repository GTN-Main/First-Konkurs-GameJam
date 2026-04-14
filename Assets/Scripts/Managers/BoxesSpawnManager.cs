using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoxesSpawnManager : MonoBehaviour, IInitializable
{
    public static BoxesSpawnManager Instance { get; private set; }

    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool[] isSpawnPointOccupied;

    bool canSpawn = false;
    [SerializeField] int startWaveSize = 3;
    [SerializeField] float spawnInterval = 4f;
    [SerializeField] List<Box> spawnedBoxes;
    int _boxesToSpawn = 0;
    bool isNowSpawningBox = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public async Task Init()
    {
        spawnedBoxes = new List<Box>();
        FindSpawnPoints();
    }

    void Update()
    {
        if (canSpawn && _boxesToSpawn > 0 && !isNowSpawningBox)
        {
            SpawnBox();
        }
        else if (canSpawn && _boxesToSpawn <= 0 && spawnedBoxes.Count == 0 && !isNowSpawningBox)
        {
            NextWave();
        }
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
            canSpawn = true;
            NextWave();
        }
        else
        {
            canSpawn = false;
        }
    }

    public void NextWave()
    {
        isSpawnPointOccupied = new bool[spawnPoints.Length];
        _boxesToSpawn = startWaveSize;
    }

    public void FindSpawnPoints()
    {
        spawnPoints = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            spawnPoints[i] = gameObject.transform.GetChild(i);
        }

        Debug.Log($"Found {spawnPoints.Length} box spawn points.");
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points found! Make sure to tag spawn point objects with 'BoxSpawnPoint'.");
        }
    }

    async Task SpawnBox()
    {
        if (canSpawn && _boxesToSpawn > 0)
        {
            isNowSpawningBox = true;
            Transform[] freeSpawnPoints = spawnPoints.Where((point, index) => !isSpawnPointOccupied[index]).ToArray();
            Transform spawnPoint = freeSpawnPoints.Length > 0 ? freeSpawnPoints[UnityEngine.Random.Range(0, freeSpawnPoints.Length)] : null;
            if (spawnPoint == null)
            {
                Debug.LogWarning("No available spawn point found!");
                isNowSpawningBox = false;
                // Wait for free spawnpoint or box to be opened
                await Task.Delay(1000); // Wait for 1 second
                return;
            }
            GameObject boxObj = Instantiate(boxPrefab, spawnPoint.position, Quaternion.identity);
            Box box = boxObj.GetComponent<Box>();
            box.name = $"Box_{startWaveSize - _boxesToSpawn + 1}";
            box.SetSpawnPoint(spawnPoint.gameObject);
            MarkSpawnPointAsOccupied(spawnPoint);
            Debug.Log($"Spawning {box.name}");
            if (box != null)
            {
                spawnedBoxes.Add(box);
                box.onBoxOpened += () =>
                {
                    Debug.Log($"Box {box.name} opened.");
                    spawnedBoxes.Remove(box);
                    Debug.Log($"Remaining boxes: {spawnedBoxes.Count}");
                    MarkSpawnPointAsFree(box.GetSpawnPoint().transform, delay: 2000f); // Delay makes sure that the box is fully opened before another one can spawn at the same point
                    Destroy(box.gameObject, 2f);
                    WaterContainerManager.Instance.AddWaterRandom();
                };
                box.BeginLandAnimation();
            }
            else
            {
                Debug.LogError("Spawned box prefab does not have a Box component!");
            }

            _boxesToSpawn--;
            await Task.Delay(Mathf.RoundToInt(spawnInterval * 1000));
            isNowSpawningBox = false;
        }
    }

    public void MarkSpawnPointAsOccupied(Transform spawnPoint)
    {
        int index = System.Array.IndexOf(spawnPoints, spawnPoint);
        if (index >= 0 && index < isSpawnPointOccupied.Length)
        {
            isSpawnPointOccupied[index] = true;
        }
    }

    public async Task MarkSpawnPointAsFree(Transform spawnPoint, float delay = 0f)
    {
        if (delay > 0f)
        {
            await Task.Delay(Mathf.RoundToInt(delay * 1000));
        }

        int index = System.Array.IndexOf(spawnPoints, spawnPoint);
        if (index >= 0 && index < isSpawnPointOccupied.Length)
        {
            isSpawnPointOccupied[index] = false;
        }
    }

    public Vector3? GetClosestBoxPositionToPoint(Vector3 point)
    {
        try
        {
            if (spawnedBoxes == null || spawnedBoxes.Count == 0)
            {
                return null;
            }

            Box closestBox = spawnedBoxes.OrderBy(box => (box.transform.position - point).sqrMagnitude).FirstOrDefault();
            return closestBox != null ? closestBox.transform.position : null;
        }
        catch
        {
            return null;
        }
    }
}
