using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerTag playerTag;
    Rigidbody2D rb;
    PlayerAttack pA;
    CapsuleCollider2D capsule;
    [SerializeField] float speed = 1000f;
    [SerializeField] bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        pA = GetComponent<PlayerAttack>();
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
            rb.simulated = false;
            pA.enabled = false;
            this.enabled = false;
        }
    }

    void FixedUpdate()
    {
        var playerInput = PlayerInputListener.Instance?.GetPlayerInput(playerTag);
        if (playerInput != null)
            Move(playerInput);

        if (playerInput != null && playerInput.attack && !isAttacking)
        {
            isAttacking = true;
            Attack();
        }
        else if (playerInput != null && !playerInput.attack)
        {
            isAttacking = false;
        }
    }

    void Move(PlayerInputData input)
    {
        Vector2 moveDirection = MovementPrinciples.GetAdjustedMovementCapsule(transform, input.direction.normalized, capsule, 0.1f, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player"));
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveDirection * speed, Time.fixedDeltaTime * 10f);
    }

    void Attack()
    {
        pA.DoAttack();
    }

    public void SetPlayerTag(PlayerTag tag)
    {
        playerTag = tag;
    }
    
    public PlayerTag GetPlayerTag() => playerTag;

    public bool IsInteracting() => PlayerInputListener.Instance?.GetPlayerInput(playerTag)?.interact ?? false;
}
