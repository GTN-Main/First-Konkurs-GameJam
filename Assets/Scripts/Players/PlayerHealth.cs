using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public void Init(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (GameManager.Instance.GetCurrentGameStateTag() == GameManager.GameStateTag.LooseGame || GameManager.Instance.GetCurrentGameStateTag() == GameManager.GameStateTag.WonGame)
            return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die(GetComponent<PlayerController>().GetPlayerTag());
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public int CurrentHealth => currentHealth;

    void Die(PlayerTag playerTag)
    {
        // Handle player death (play animation, disable controls, etc.)
        Debug.Log("One player has died.");
        OnPlayerDied?.Invoke(playerTag);
    }
    
    public event System.Action<PlayerTag> OnPlayerDied;
}
