using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject paintballPrefab;

    [SerializeField]
    private Vector2 randomDirectionRangeInDegrees = new Vector2(-15f, 15f);

    [SerializeField]
    private Vector2 randomMovementDistanceMultiplierRange = new Vector2(0.7f, 1.3f);

    [SerializeField]
    private float attackCooldown = 2f;
    private float lastAttackTime = -Mathf.Infinity;

    [SerializeField]
    private float maxAttackDistance = 5f;

    /// <summary>
    /// Returns true if the attack was successfully initiated, false if it is still on cooldown
    /// </summary>
    public bool DoAttack(GameObject target)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return false;
        }

        lastAttackTime = Time.time;

        Paintball paintball = CreatePaintball();
        if (paintball != null)
        {
            Vector2 targetDirection =
                target != null
                    ? (target.transform.position - transform.position).normalized
                    : Vector2.zero;
            targetDirection =
                Quaternion.Euler(
                    0,
                    0,
                    Random.Range(randomDirectionRangeInDegrees.x, randomDirectionRangeInDegrees.y)
                ) * targetDirection;
            float distanceToEnemy =
                target != null
                    ? Vector2.Distance(transform.position, target.transform.position)
                    : 0f;
            distanceToEnemy =
                distanceToEnemy > 0f
                    ? Mathf.Min(
                        distanceToEnemy
                            * UnityEngine.Random.Range(
                                randomMovementDistanceMultiplierRange.x,
                                randomMovementDistanceMultiplierRange.y
                            ),
                        maxAttackDistance
                    )
                    : 0f;
            paintball.Init(
                transform.position,
                explosionRadius: 2f,
                explosionDamage: 10,
                movementVector: targetDirection,
                movementDistance: distanceToEnemy,
                movementTime: 1.2f,
                damageableLayers: LayerMask.GetMask("Players")
            );

            paintball.Run();
            MyAudioEffects.Instance.DoEffect("Throw", transform.position, 1f);
        }
        else
        {
            Debug.LogError(
                "Paintball prefab does not have Paintball component. or paintballPrefab reference is null."
            );
        }

        return true;
    }

    private Paintball CreatePaintball()
    {
        return Instantiate(paintballPrefab, transform.position, Quaternion.identity)
            .GetComponent<Paintball>();
    }
}
