using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action OnDeath;
    private float maxHealth;
    private float currentHealth;

    public void Init(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
