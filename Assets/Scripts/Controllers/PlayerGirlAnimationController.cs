using System.Collections.Generic;
using UnityEngine;

public class PlayerGirlAnimationController : MovableAnimationController
{
    [SerializeField] MovementPrinciples.MovableDirection currentDirection = MovementPrinciples.MovableDirection.None;
    [SerializeField] AnimationClip UpWalk, DownWalk, LeftWalk, RightWalk, Stand;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 velocityVector;
    bool isGameOver = false;

    void Start()
    {
        Init(GetComponent<Animator>(), new Dictionary<string, AnimationClip>()
            {
                { "Up", UpWalk },
                { "Down", DownWalk },
                { "Left", LeftWalk },
                { "Right", RightWalk },
                { "None", Stand }
            }
        );

        ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));
        isGameOver = false;
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
            isGameOver = true;
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;
        Vector2 movementVector = GetVelocityVector();
        velocityVector = new Vector2(Mathf.Round(movementVector.x), Mathf.Round(movementVector.y));
        var movableDirection = MovementPrinciples.GetDirectionFromMoveVector(velocityVector);

        if (movableDirection != currentDirection)
        {
            currentDirection = movableDirection;
            ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));
        }

        UpdateController();
    }

    Vector2 GetVelocityVector()
    {
        return rb.linearVelocity;
    }
}
