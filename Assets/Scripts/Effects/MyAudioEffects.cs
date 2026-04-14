using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyAudioEffects : MonoBehaviour, IInitializable
{
    public static MyAudioEffects Instance { get; private set; }

    [SerializeField]
    private AudioClip[] audioClips;
    private Dictionary<string, AudioClip> prefabDictionary;

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
        audioClips = Resources.LoadAll<AudioClip>("Prefabs/AudioEffects/");
        Debug.Log($"Loaded {audioClips.Length} audio clips.");
    }

    void MakeDictionary()
    {
        prefabDictionary = new Dictionary<string, AudioClip>();
        foreach (var audioClip in audioClips)
        {
            prefabDictionary[audioClip.name] = audioClip;
            Debug.Log($"Added audio clip '{audioClip.name}' to dictionary.");
        }
    }

    public void DoEffect(string effectName, Vector3 position, float volume = 1f)
    {
        SpawnAudioEffect(effectName, position, volume);
    }

    private void SpawnAudioEffect(string effectName, Vector3 position, float volume)
    {
        if (prefabDictionary == null)
        {
            Debug.LogError("Prefab dictionary is not initialized.");
            return;
        }

        if (prefabDictionary.TryGetValue(effectName, out AudioClip audioClip))
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volume: volume);
        }
        else
        {
            Debug.LogError($"Audio effect '{effectName}' not found in dictionary.");
        }
    }
}
