using System.Threading.Tasks;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour, IInitializable
{
    [SerializeField]
    GameObject playerPrefabGirl;

    [SerializeField]
    GameObject playerPrefabFasolka;

    public PlayerController Player1,
        Player2;

    public static PlayerSpawnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (playerPrefabGirl == null)
        {
            Debug.LogError("Player prefab for PlayerGirl is not assigned in the inspector.");
        }

        if (playerPrefabFasolka == null)
        {
            Debug.LogError("Player prefab for PlayerFasolka is not assigned in the inspector.");
        }
    }

    public async Task Init()
    {
        await SpawnPlayers();
    }

    public void ResetPlayers()
    {
        if (Player1 != null)
        {
            Destroy(Player1.gameObject);
            Player1 = null;
        }
        if (Player2 != null)
        {
            Destroy(Player2.gameObject);
            Player2 = null;
        }
        _ = SpawnPlayers();
    }

    async Task SpawnPlayers()
    {
        await SpawnPlayer(PlayerTag.Player1);
        await SpawnPlayer(PlayerTag.Player2);
    }

    async Task SpawnPlayer(PlayerTag playerTag)
    {
        Vector3 spawnPosition =
            playerTag == PlayerTag.Player1 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0);
        var player = Instantiate(
            playerTag == PlayerTag.Player1 ? playerPrefabFasolka : playerPrefabGirl,
            spawnPosition,
            Quaternion.identity
        );
        player.name = playerTag.ToString();
        player.layer = LayerMask.NameToLayer("Players");
        player.transform.SetParent(transform);

        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetPlayerTag(playerTag);
            if (playerTag == PlayerTag.Player1)
                Player1 = playerController;
            else
                Player2 = playerController;
        }

        var pGAC = player.transform.GetChild(0).GetComponent<PlayerGirlAnimationController>();
        if (pGAC != null)
        {
            pGAC.SetPlayerTag(playerTag);
        }

        var fAC = player.transform.GetChild(0).GetComponent<PlayerFasolkaAnimationController>();
        if (fAC != null)
        {
            fAC.SetPlayerTag(playerTag);
        }

        await Task.Yield();
    }
}
