using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    float attackCooldown = 1f;
    [SerializeField]
    float separationStrength = 1.5f;
    Rigidbody2D rb;
    CapsuleCollider2D capsule;
    EnemyHealth health;

    [SerializeField]
    float separationTimerInterval = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        health = GetComponent<EnemyHealth>();
        health.OnDeath += Die;
    }
    Vector2 separationVector = Vector2.zero;
    Vector2 desiredVelocity = Vector2.zero;
    Vector2 _localdesiredVelocity = Vector2.zero;
    void FixedUpdate()
    {
        /*separationTimer += Time.deltaTime;
        if (separationTimer >= separationTimerInterval)
        {
            separationTimer = 0f;
            Vector2 newSeparationVector = MovementPrinciples.GetRepulsionWeight(transform, 2f, LayerMask.GetMask("Enemies"));
            if (newSeparationVector.sqrMagnitude < 0.01f) newSeparationVector = Vector2.zero;
            separationVector = Vector2.Lerp(separationVector, newSeparationVector.normalized, Time.deltaTime * 2f);
        }
        Debug.DrawRay(transform.position, separationVector * separationStrength, Color.white);*/

        Vector2 velocityNormalized = _localdesiredVelocity.normalized;
        Vector2 finalVelocity = MovementPrinciples.GetAdjustedMovementCapsule(transform, velocityNormalized, capsule, 0.05f, LayerMask.GetMask("Obstacle")) * _localdesiredVelocity.magnitude;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, finalVelocity, Time.fixedDeltaTime * 10f);
        Debug.DrawRay(transform.position, _localdesiredVelocity, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity, Color.blue);
        Debug.DrawRay(transform.position, finalVelocity, Color.crimson);

        _localdesiredVelocity = Vector2.Lerp(_localdesiredVelocity, desiredVelocity, Time.fixedDeltaTime * 15f);
    }

    public void Move(Vector2 desiredVelocity)
    {        
        this.desiredVelocity = desiredVelocity;
    }

    public void MakeAttack(GameObject target)
    {
        //sth
        Debug.Log($"Attacking {target.name}");
    }

    public (bool, float) GetAttackCooldown()
    {
        return (true, attackCooldown);
    }

    [ContextMenu("Kill")]
    public void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void InitHealth(float health)
    {
        if (this.health == null)
        {
            Debug.LogError("EnemyHealth component is missing!");
            return;
        }
        
        this.health.Init(health);
    }

    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
    }

    public event Action OnDeath;
}
