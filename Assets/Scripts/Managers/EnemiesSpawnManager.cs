using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemiesSpawnManager : MonoBehaviour, IInitializable
{
    public static EnemiesSpawnManager Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] bool canSpawn = false;
    [SerializeField] int waveNumber = 0;
    [SerializeField] int startWaveSize = 3;
    [SerializeField] float nextWaveSizeGrowth = 1.2f;
    [SerializeField] int currentWaveSize = 0;
    [SerializeField] float spawnInterval = 1f;
    [SerializeField] List<Enemy> spawnedEnemies;
    [SerializeField] int _enemiesToSpawn = 0;
    [SerializeField] bool isNowSpawningEnemy = false;

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
        spawnedEnemies = new List<Enemy>();
        FindSpawnPoints();
    }

    void Update()
    {
        if (currentWaveSize == 0) return;
        
        if (canSpawn && _enemiesToSpawn > 0 && !isNowSpawningEnemy)
        {
            SpawnEnemy();
        }
        else if (canSpawn && _enemiesToSpawn <= 0 && spawnedEnemies.Count == 0 && !isNowSpawningEnemy)
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
            waveNumber = 0;
            NextWave();
        }
        else
        {
            canSpawn = false;
        }
    }

    public void NextWave()
    {
        Debug.Log($"Wave {waveNumber} completed!");
        if (waveNumber == 0)
            currentWaveSize = startWaveSize;
        else
            currentWaveSize = Mathf.RoundToInt(currentWaveSize * nextWaveSizeGrowth);

        _enemiesToSpawn = currentWaveSize;
        waveNumber++;
    }

    public void FindSpawnPoints()
    {
        spawnPoints = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            spawnPoints[i] = gameObject.transform.GetChild(i);
        }

        Debug.Log($"Found {spawnPoints.Length} enemy spawn points.");
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points found! Make sure to tag spawn point objects with 'EnemySpawnPoint'.");
        }
    }

    async Task SpawnEnemy()
    {
        if (canSpawn && _enemiesToSpawn > 0)
        {
            isNowSpawningEnemy = true;
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.name = $"Enemy_{waveNumber}_{currentWaveSize - _enemiesToSpawn + 1}";
            Debug.Log($"Spawning {enemy.name}");
            if (enemy != null)
            {
                enemy.InitHealth(10f);
                spawnedEnemies.Add(enemy);
                enemy.OnDeath += () => { Debug.Log($"Enemy {enemy.name} died."); spawnedEnemies.Remove(enemy); Debug.Log($"Remaining enemies: {spawnedEnemies.Count}"); };
            }
            else
            {
                Debug.LogError("Spawned enemy prefab does not have an Enemy component!");
            }

            _enemiesToSpawn--;
            await Task.Delay(Mathf.RoundToInt(spawnInterval * 1000));
            isNowSpawningEnemy = false;
        }
        else
        {
            Debug.LogWarning("Cannot spawn enemy: either spawning is disabled or there are no enemies left to spawn.");
        }
    }

    public Enemy GetClosestEnemyToPoint(Vector2 position)
    {
        Enemy closerEnemy = null;
        float closerDistanceSqr = float.MaxValue;

        foreach (var enemy in spawnedEnemies)
        {
            float distanceSqr = (enemy.transform.position - (Vector3)position).sqrMagnitude;
            if (distanceSqr < closerDistanceSqr)
            {
                closerDistanceSqr = distanceSqr;
                closerEnemy = enemy;
            }
        }

        return closerEnemy;
    }
}
