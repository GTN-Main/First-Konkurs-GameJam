using UnityEngine;

public class MovementPrinciples
{
    public static Vector2 GetAdjustedMovement(Transform transform, Vector2 moveInput, Vector2 rayDirection, float distanceToWall, LayerMask wallLayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection.normalized, distanceToWall, wallLayer);

        if (hit.collider == null)
        {
            return moveInput;
        }

        Vector2 wallNormal = hit.normal;

        // check the pushforce coefficient against the wall normal using the dot product
        float dot = Vector2.Dot(moveInput, wallNormal);

        // if the dot product is negative, it means the movement is towards the wall, so we need to adjust it
        if (dot < 0)
        {
            // we want to push the movement away from the wall, so we subtract the component of the movement that is towards the wall
            // the result is the direction parrallel to the wall, which allows the character to slide along it instead of getting stuck
            return moveInput - dot * wallNormal;
        }

        return moveInput;
    }

    public static Vector2 GetAdjustedMovementCapsule(
    Transform transform,
    Vector2 moveInput,
    CapsuleCollider2D capsule,
    float extraDistance,
    LayerMask wallLayer,
    float intencity = 1)
    {
        Vector2 currentVelocity = moveInput;

        Vector2 size = capsule.size;
        CapsuleDirection2D direction = capsule.direction;
        float angle = transform.eulerAngles.z;

        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(
                transform.position + (Vector3)capsule.offset,
                size,
                direction,
                angle,
                currentVelocity.normalized,
                extraDistance,
                wallLayer
            );

            if (hit.collider == null)
                break;

            Vector2 wallNormal = hit.normal;
            float dot = Vector2.Dot(currentVelocity, wallNormal);

            if (dot < 0)
            {
                currentVelocity -= dot * wallNormal * intencity;
            }

            if (currentVelocity.sqrMagnitude < 0.001f)
            {
                currentVelocity = Vector2.zero;
                break;
            }
        }

        return currentVelocity.normalized * moveInput.magnitude;
    }

    public static Vector2 GetRepulsionWeight(Transform transform, float radius, LayerMask entityLayer)
    {
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, radius, entityLayer);

        Vector2 repulsionForce = Vector2.zero;

        foreach (var other in overlaps)
        {
            if (other.transform == transform) continue;

            // Calculating the direction from the other entity to the current one
            Vector2 diff = (Vector2)transform.position - (Vector2)other.transform.position;

            // The closer we get, the more we push away
            // The force grows exponentially as we get closer
            float distance = diff.magnitude;
            if (distance < 0.001f) continue; // Unikamy dzielenia przez zero

            repulsionForce += diff.normalized / distance;
        }

        return repulsionForce;
    }

    public static MovableDirection GetDirectionFromMoveVector(Vector2 movement)
    {
        const float deadzone = 0.1f;

        if (movement.sqrMagnitude < deadzone * deadzone)
            return MovableDirection.None;

        // Vertical
        if (Mathf.Abs(movement.y) >= Mathf.Abs(movement.x))
        {
            if (movement.y > 0f)
                return MovableDirection.Up;
            else
                return MovableDirection.Down;
        }
        // Horizontal
        else
        {
            if (movement.x > 0f)
                return MovableDirection.Right;
            else
                return MovableDirection.Left;
        }
    }

    public static string MovableDirectionToString(MovableDirection direction)
    {
        switch (direction)
        {
            case MovableDirection.None:
                return "None";
            case MovableDirection.Right:
                return "Right";
            case MovableDirection.Left:
                return "Left";
            case MovableDirection.Up:
                return "Up";
            case MovableDirection.Down:
                return "Down";
        }
        return "None";
    }

    public enum MovableDirection
    {
        None,
        Right,
        Left,
        Up,
        Down
    }
}


