using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour, IInitializable
{
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    Color player1Color = Color.blue, player2Color = Color.red;

    public PlayerController Player1, Player2;

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

    public void Init()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        SpawnPlayer(PlayerTag.Player1);
        SpawnPlayer(PlayerTag.Player2);
    }

    void SpawnPlayer(PlayerTag playerTag)
    {
        Vector3 spawnPosition = playerTag == PlayerTag.Player1 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0);
        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
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

        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = playerTag == PlayerTag.Player1 ? player1Color : player2Color;
        }
    }
}
