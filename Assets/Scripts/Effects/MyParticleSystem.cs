using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour, IInitializable
{
    public static MyParticleSystem Instance { get; private set; }
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<string, GameObject> prefabDictionary;

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

    public async Task Init()
    {
        LoadPrefabs();
        MakeDictionary();
    }

    void LoadPrefabs()
    {
        prefabs = Resources.LoadAll<GameObject>("Prefabs/Effects/");
        Debug.Log($"Loaded {prefabs.Length} particle system prefabs.");
    }

    void MakeDictionary()
    {
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (var prefab in prefabs)
        {
            prefabDictionary[prefab.name] = prefab;
            Debug.Log($"Added prefab '{prefab.name}' to dictionary.");
        }
    }

    public void DoEffect(string effectName, Vector3 position, float timeToLive)
    {
        SpawnParticleEffect(effectName, position, timeToLive);
    }

    private void SpawnParticleEffect(string effectName, Vector3 position, float timeToLive)
    {
        if (prefabDictionary == null)
        {
            Debug.LogError("Prefab dictionary is not initialized.");
            return;
        }
        
        if (prefabDictionary.TryGetValue(effectName, out GameObject prefab))
        {
            var effect = Instantiate(prefab, position, Quaternion.identity);
            effect.transform.SetParent(transform);
            effect.transform.localScale = Vector3.one;
            Destroy(effect, timeToLive);
            Debug.Log($"Spawned particle effect '{effectName}' at {position}.");
        }
        else
        {
            Debug.LogError($"Particle effect '{effectName}' not found in dictionary.");
        }
    }
}
