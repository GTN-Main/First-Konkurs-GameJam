using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerTag playerTag;
    Rigidbody2D rb;
    CapsuleCollider2D capsule;
    [SerializeField] float speed = 1000f;
    
    public void SetPlayerTag(PlayerTag tag)
    {
        playerTag = tag;
    }

    public bool IsInteracting() => PlayerInputListener.Instance?.GetPlayerInput(playerTag)?.interact ?? false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        var playerInput = PlayerInputListener.Instance?.GetPlayerInput(playerTag);
        if (playerInput != null)
            Move(playerInput);
    }

    void Move(PlayerInputData input)
    {
        Vector2 moveDirection = MovementPrinciples.GetAdjustedMovementCapsule(transform, input.direction.normalized, capsule, 0.1f, LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player"));
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, moveDirection * speed, Time.fixedDeltaTime * 10f);
    }
}
