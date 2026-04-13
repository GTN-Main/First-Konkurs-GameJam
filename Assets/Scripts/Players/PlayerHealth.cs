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
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
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

    void Die()
    {
        // Handle player death (play animation, disable controls, etc.)
        Debug.Log("One player has died.");
        OnPlayerDied?.Invoke();
    }
    
    public event System.Action OnPlayerDied;
}
