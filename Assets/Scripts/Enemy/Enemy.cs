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
    EnemyAttack enemyAttack;
    EnemyMovementStateMachine movementStateMachine;
    [SerializeField]
    FasolkaAnimationController fasolkaAnimationController;

    [SerializeField]
    float separationTimerInterval = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        health = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
            Debug.LogError("EnemyAttack component is missing!");
        movementStateMachine = GetComponent<EnemyMovementStateMachine>();
        if (movementStateMachine == null)
            Debug.LogError("EnemyMovementStateMachine component is missing!");
        if (fasolkaAnimationController == null)
            Debug.LogError("FasolkaAnimationController component is missing!");
        health.OnDeath += Die;
    }

    void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state.GetTag() == GameManager.GameStateTag.LooseGame || state.GetTag() == GameManager.GameStateTag.WonGame)
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        rb.simulated = false;
        this.enabled = false;
        movementStateMachine.enabled = false;
        enemyAttack.enabled = false;
    }

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
        //Vector2 finalVelocity = MovementPrinciples.GetAdjustedMovementCapsule(transform, velocityNormalized, capsule, 0.05f, LayerMask.GetMask("Obstacle")) * _localdesiredVelocity.magnitude;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, _localdesiredVelocity, Time.fixedDeltaTime * 15f);
        Debug.DrawRay(transform.position, _localdesiredVelocity, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity, Color.blue);
        //Debug.DrawRay(transform.position, finalVelocity, Color.crimson);

        _localdesiredVelocity = Vector2.Lerp(_localdesiredVelocity, desiredVelocity, Time.fixedDeltaTime * 15f);
    }

    public void Move(Vector2 desiredVelocity)
    {        
        this.desiredVelocity = desiredVelocity;
    }

    public void MakeAttack(GameObject target)
    {
        enemyAttack.DoAttack(target);
        fasolkaAnimationController.DoAttackAnim();
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
