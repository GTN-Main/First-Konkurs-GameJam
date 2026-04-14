using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MovableAnimationController
{
    [SerializeField] MovementPrinciples.MovableDirection currentDirection = MovementPrinciples.MovableDirection.None;
    [SerializeField] AnimationClip UpWalk, DownWalk, LeftWalk, RightWalk;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 velocityVector;
    [SerializeField] SpriteRenderer mainBodySpriteRenderer;
    [SerializeField] Sprite[] variants_front;
    [SerializeField] Sprite[] variants_back;
    [SerializeField] int variantIndex = 0;
    bool isGameOver = false;

    void Start()
    {
        Init(GetComponent<Animator>(), new Dictionary<string, AnimationClip>()
            {
                { "Up", UpWalk },
                { "Down", DownWalk },
                { "Left", LeftWalk },
                { "Right", RightWalk },
                { "None", DownWalk }
            }
        );

        ChangeAnim(MovementPrinciples.MovableDirectionToString(currentDirection));

        if (variants_front.Length == 0 || variants_back.Length == 0)
        {
            Debug.LogWarning("Variants arrays are empty.");
        }
        else if (variants_front.Length != variants_back.Length)
        {
            Debug.LogWarning("Front and back variants arrays must be of the same length.");
        }

        variantIndex = Random.Range(0, variants_front.Length);
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
            SetVariant();
        }

        UpdateController();
    }

    private void SetVariant()
    {
        mainBodySpriteRenderer.sprite = currentDirection == MovementPrinciples.MovableDirection.Up ? variants_back[variantIndex] : variants_front[variantIndex];
    }

    Vector2 GetVelocityVector()
    {
        return rb.linearVelocity;
    }
}
