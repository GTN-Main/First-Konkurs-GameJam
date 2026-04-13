using UnityEngine;

public class HealthManager : MonoBehaviour, IInitializable
{
    public static HealthManager Instance { get; private set; }
    [SerializeField] private PlayerHealthUI player1HealthUI;
    
    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        
    }
}
