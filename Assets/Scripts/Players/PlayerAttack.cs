using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject granadePrefab;

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
    public bool DoAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return false;
        }

        lastAttackTime = Time.time;

        GrenadeExplode granadeEx = CreateGranade();
        if (granadeEx != null)
        {
            Enemy theClosestEnemy = EnemiesSpawnManager.Instance.GetClosestEnemyToPoint(
                transform.position
            );
            Debug.Log(
                $"Closest enemy to player is {(theClosestEnemy != null ? theClosestEnemy.name : "none")}"
            );
            Vector2 targetDirection =
                theClosestEnemy != null
                    ? (theClosestEnemy.transform.position - transform.position).normalized
                    : Vector2.zero;
            targetDirection =
                Quaternion.Euler(
                    0,
                    0,
                    Random.Range(randomDirectionRangeInDegrees.x, randomDirectionRangeInDegrees.y)
                ) * targetDirection;
            float distanceToEnemy =
                theClosestEnemy != null
                    ? Vector2.Distance(transform.position, theClosestEnemy.transform.position)
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
            granadeEx.Init(
                transform.position,
                explosionRadius: 5f,
                explosionDamage: 50f,
                explosionDelay: 2f,
                movementVector: targetDirection,
                movementDistance: distanceToEnemy,
                movementTime: 1f,
                damageableLayers: LayerMask.GetMask("Enemies")
            );

            granadeEx.Run();
        }
        else
        {
            Debug.LogError(
                "Granade prefab does not have GrenadeExplode component. or granadePrefab reference is null."
            );
        }

        return true;
    }

    private GrenadeExplode CreateGranade()
    {
        return Instantiate(granadePrefab, transform.position, Quaternion.identity)
            .GetComponent<GrenadeExplode>();
    }
}
