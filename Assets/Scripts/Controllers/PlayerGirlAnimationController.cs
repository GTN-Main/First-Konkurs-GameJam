using System.Collections.Generic;
using UnityEngine;

public class PlayerGirlAnimationController : MovableAnimationController
{
    [SerializeField] MovementPrinciples.MovableDirection currentDirection = MovementPrinciples.MovableDirection.None;
    [SerializeField] AnimationClip UpWalk, DownWalk, LeftWalk, RightWalk, Stand;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 velocityVector;

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
    }

    void FixedUpdate()
    {
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
