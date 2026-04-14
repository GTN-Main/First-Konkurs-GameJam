using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour, IInitializable
{
    public static HealthManager Instance { get; private set; }
    [SerializeField] int maxHealth = 50;
    [SerializeField] int heartsUIHealthCount = 5;

    private PlayerHealth player1Health, player2Health;

    [SerializeField] private PlayerHealthUI player1HealthUI, player2HealthUI;
    [SerializeField] private GameObject panelPlayer1Health, panelPlayer2Health;
    bool opened = false;

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

    public async Task Init()
    {
        HideHealthUI();
    }

    void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        player1Health.OnPlayerDied -= OnOnePlayerDied;
        player2Health.OnPlayerDied -= OnOnePlayerDied;
        player1Health = null;
        player2Health = null;
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        opened = false;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state.GetTag() == GameManager.GameStateTag.StartGame && !opened)
        {
            opened = true;
            OnGameStart();
        }

        if (!opened) return;
        
        if (state.GetTag() == GameManager.GameStateTag.StartGame)
        {
            ShowHealthUI();
        }
        else
        {
            HideHealthUI();
        }
    }

    public void ShowHealthUI()
    {
        panelPlayer1Health.SetActive(true);
        panelPlayer2Health.SetActive(true);
    }

    public void HideHealthUI()
    {
        panelPlayer1Health.SetActive(false);
        panelPlayer2Health.SetActive(false);
    }

    [ContextMenu("Damage Player 1 for testing")]
    public void DamagePlayer1ForTesting()
    {
        DamagePlayer(PlayerTag.Player1, 5);
    }

    [ContextMenu("Heal Player 1 for testing")]
    public void HealPlayer1ForTesting()
    {
        HealPlayer(PlayerTag.Player1, 5);
    }

    public void HealPlayer(PlayerTag playerTag, int amount)
    {
        if (playerTag == PlayerTag.Player1)
        {
            player1Health.Heal(amount);
            UpdateHealthUI1();
        }
        else if (playerTag == PlayerTag.Player2)
        {
            player2Health.Heal(amount);
            UpdateHealthUI2();
        }
    }

    public void DamagePlayer(PlayerTag playerTag, int damage)
    {
        if (playerTag == PlayerTag.Player1)
        {
            player1Health.TakeDamage(damage);
            if (player1Health.CurrentHealth > 0)
            {
                UpdateHealthUI1();
            }
            else
            {
                UpdateHealthUI1(0);
            }
        }
        else if (playerTag == PlayerTag.Player2)
        {
            player2Health.TakeDamage(damage);
            if (player2Health.CurrentHealth > 0)
            {
                UpdateHealthUI2();
            }
            else
            {
                UpdateHealthUI2(0);
            }
        }
    }

    public void UpdateHealthUI1(int forceHearts =  -1)
    {
        DebugUtility.WriteInColor($"Updating Player 1 Health UI. Hearts: {forceHearts}", Color.yellow);
        int player1HealthHearts = forceHearts >= 0 ? forceHearts : Mathf.Clamp(Mathf.CeilToInt((float)player1Health.CurrentHealth / maxHealth * heartsUIHealthCount), 0, heartsUIHealthCount);

        player1HealthUI.ShowHealthUI(player1HealthHearts);
    }

    public void UpdateHealthUI2(int forceHearts =  -1)
    {
        DebugUtility.WriteInColor($"Updating Player 2 Health UI. Hearts: {forceHearts}", Color.yellow);
        int player2HealthHearts = forceHearts >= 0 ? forceHearts : Mathf.Clamp(Mathf.CeilToInt((float)player2Health.CurrentHealth / maxHealth * heartsUIHealthCount), 0, heartsUIHealthCount);

        player2HealthUI.ShowHealthUI(player2HealthHearts);
    }

    public void OnGameStart()
    {
        player1Health = PlayerSpawnManager.Instance?.Player1.GetComponent<PlayerHealth>();
        player2Health = PlayerSpawnManager.Instance?.Player2.GetComponent<PlayerHealth>();

        if (player1Health == null || player2Health == null)
        {
            Debug.LogError("PlayerHealth component not found on one or both of the players.");
            return;
        }

        player1Health.Init(maxHealth);
        player2Health.Init(maxHealth);

        player1HealthUI.Init(heartsUIHealthCount);
        player2HealthUI.Init(heartsUIHealthCount);

        player1Health.OnPlayerDied += OnOnePlayerDied;
        player2Health.OnPlayerDied += OnOnePlayerDied;
    }

    public void OnOnePlayerDied(PlayerTag playerTag)
    {
        // Handle game over logic (e.g., show game over screen, reset the game, etc.)
        player1Health.OnPlayerDied -= OnOnePlayerDied;
        player2Health.OnPlayerDied -= OnOnePlayerDied;
        player1Health = null;
        player2Health = null;
        opened = false;

        if (playerTag == PlayerTag.Player1)
        {
            UpdateHealthUI1(0);
        }
        else if (playerTag == PlayerTag.Player2)
        {
            UpdateHealthUI2(0);
        }
        GameManager.Instance?.LooseGame();
    }
}
