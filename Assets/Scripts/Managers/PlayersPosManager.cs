using UnityEngine;

public class PlayersPosManager : MonoBehaviour
{
    public static PlayersPosManager Instance { get; private set; }

    public void Awake()
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

    public GameObject GetClosestPlayerToPoint(Vector2 point)
    {
        if (PlayerSpawnManager.Instance.Player1 == null || PlayerSpawnManager.Instance.Player2 == null)
            return this.gameObject; // Return self if any of the players is missing
        float sqrDistanceToPlayer1 = Vector2.SqrMagnitude(PlayerSpawnManager.Instance.Player1.transform.position - (Vector3)point);
        float sqrDistanceToPlayer2 = Vector2.SqrMagnitude(PlayerSpawnManager.Instance.Player2.transform.position - (Vector3)point);
        if (sqrDistanceToPlayer1 < sqrDistanceToPlayer2)
            return PlayerSpawnManager.Instance.Player1.gameObject;
        else
            return PlayerSpawnManager.Instance.Player2.gameObject;
    }
}
