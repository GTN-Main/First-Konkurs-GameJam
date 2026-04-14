using System.Collections.Generic;
using UnityEngine;

public class PlayerFasolkaAnimationController : MovableAnimationController
{
    [SerializeField] PlayerTag playerTag;
    [SerializeField] MovementPrinciples.MovableDirection currentDirection = MovementPrinciples.MovableDirection.None;
    [SerializeField] AnimationClip UpWalk, DownWalk, LeftWalk, RightWalk, Stand, UpAttack, DownAttack, LeftAttack, RightAttack;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 velocityVector;
    [SerializeField] SpriteRenderer mainBodySpriteRenderer;
    [SerializeField] Sprite main_front;
    [SerializeField] Sprite main_back;
    bool isGameOver = false;
    bool isAttacking = false;
    bool lastAttackState = false;
    float attackButtonHoldTime = 0f;
    float maxAttackHoldTime = 0.2f;

    void Start()
    {
        Init(GetComponent<Animator>(), new Dictionary<string, AnimationClip>()
            {
                { "Up", UpWalk },
                { "Down", DownWalk },
                { "Left", LeftWalk },
                { "Right", RightWalk },
                { "None", Stand },
                { "UpAttack", UpAttack },
                { "DownAttack", DownAttack },
                { "LeftAttack", LeftAttack },
                { "RightAttack", RightAttack },
                { "NoneAttack", DownAttack },
            }
        );

        ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));
        isGameOver = false;
    }

    public void SetPlayerTag(PlayerTag tag) => playerTag = tag;

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
            isGameOver = true;
        }
    }

    void Update()
    {
        var playerInput = PlayerInputListener.Instance?.GetPlayerInput(playerTag);
        if (playerInput != null)
        {
            if (playerInput.attack)
            {
                attackButtonHoldTime += Time.deltaTime;
                if (attackButtonHoldTime < maxAttackHoldTime)
                    isAttacking = true;
                else
                    isAttacking = false;
            }
            else
            {
                isAttacking = false;
                attackButtonHoldTime = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;
        Vector2 movementVector = GetVelocityVector();
        velocityVector = new Vector2(Mathf.Round(movementVector.x), Mathf.Round(movementVector.y));
        var movableDirection = MovementPrinciples.GetDirectionFromMoveVector(velocityVector);

        if (lastAttackState != isAttacking)
        {
            lastAttackState = isAttacking;
            if (isAttacking)
            {
                ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection) + "Attack");
            }
            else
            {
                ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));
            }
        }
        else if (movableDirection != currentDirection)
        {
            currentDirection = movableDirection;
            ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));
        }

        UpdateController();
        SetVariant();
    }

    private void SetVariant()
    {
        mainBodySpriteRenderer.sprite = currentDirection == MovementPrinciples.MovableDirection.Up ? main_back : main_front;
    }


    Vector2 GetVelocityVector()
    {
        return rb.linearVelocity;
    }
}
