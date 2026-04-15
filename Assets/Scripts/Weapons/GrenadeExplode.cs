using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class GrenadeExplode : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;

    [SerializeField]
    private float explosionDamage = 50f;

    [SerializeField]
    Vector2 movementVector = Vector2.zero;

    [SerializeField]
    private float movementDistance = 5f;

    [SerializeField]
    private float movementTime = 5f;

    [SerializeField]
    private LayerMask damageableLayers;

    [SerializeField]
    private Transform maxRadiusImageT;

    [SerializeField]
    private Transform fillImageT;

    [SerializeField]
    private Vector2 aspectRatioSize;

    [SerializeField]
    AnimationCurve moveOverTimeCurve;

    [SerializeField]
    private Transform flameImageT;

    [SerializeField]
    private Vector2 flameStartPos;

    [SerializeField]
    private Vector2 flameEndPos;

    Rigidbody2D rb;

    Vector2 startPosition;

    public void Init(
        Vector2 position,
        float explosionRadius,
        float explosionDamage,
        Vector2 movementVector,
        float movementDistance,
        float movementTime,
        LayerMask damageableLayers
    )
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = position;
        startPosition = position;
        this.explosionRadius = explosionRadius;
        this.explosionDamage = explosionDamage;
        this.movementVector = movementVector;
        this.movementDistance = movementDistance;
        this.movementTime = movementTime;
        this.damageableLayers = damageableLayers;
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
        if (
            state.GetTag() == GameManager.GameStateTag.LooseGame
            || state.GetTag() == GameManager.GameStateTag.WonGame
        )
        {
            Destroy(gameObject);
        }
    }

    public void Run()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        StartCoroutine(MoveRoutine());
        VisualizeExplosionRadius();
        VisualizeFlame();
        Explode();
    }

    public void OnDrawGizmos()
    {
        Vector2 targetPosition = startPosition + movementVector.normalized * movementDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, explosionRadius);
        Gizmos.DrawWireSphere(
            targetPosition,
            explosionRadius * aspectRatioSize.y / aspectRatioSize.x
        );
    }

    IEnumerator MoveRoutine()
    {
        float elapsedTime = 0f;
        Vector2 targetPosition = startPosition + movementVector.normalized * movementDistance;
        Debug.Log(
            $"Moving grenade from {startPosition} to {targetPosition} over {movementTime} seconds."
        );

        rb.WakeUp();

        while (elapsedTime < movementTime)
        {
            elapsedTime += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / movementTime);
            float curveT = moveOverTimeCurve.Evaluate(t);

            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, curveT);

            rb.MovePosition(newPosition);
            Debug.Log($"Moving grenade to {newPosition} with velocity {rb.linearVelocity}");

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
    }

    async Task Explode()
    {
        var c = this.AddComponent<CapsuleCollider2D>();
        c.direction = CapsuleDirection2D.Horizontal;
        c.size = new Vector2(
            explosionRadius * 2f + 0.1f,
            explosionRadius * 2f * aspectRatioSize.y / aspectRatioSize.x + 0.1f
        );
        c.isTrigger = true;
        await Task.Delay(Mathf.RoundToInt(movementTime * 1000));
        Collider2D[] hitColliders = Physics2D.OverlapCapsuleAll(
            transform.position,
            new Vector2(
                explosionRadius * 2f + 0.1f,
                explosionRadius * 2f * aspectRatioSize.y / aspectRatioSize.x + 0.1f
            ),
            CapsuleDirection2D.Horizontal,
            0,
            damageableLayers
        );
        foreach (Collider2D hitCollider in hitColliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);
            }
        }
        MyParticleSystem mP = MyParticleSystem.Instance;
        if (mP == null)
            Debug.LogError("MyParticleSystem instance is null!");
        MyParticleSystem.Instance.DoEffect("Boom", transform.position, 1f);
        MyAudioEffects.Instance.DoEffect("GrenadeExplosion", transform.position, 1f);
        Destroy(gameObject);
    }

    async Task VisualizeExplosionRadius()
    {
        if (maxRadiusImageT == null)
        {
            Debug.LogError("Max radius object is null");
        }

        if (fillImageT == null)
        {
            Debug.LogError("Fill object is null");
        }

        float elapsedTime = 0f;
        float maxDiameter = explosionRadius * 2f;

        maxRadiusImageT.localScale = Vector2.one * aspectRatioSize * maxDiameter;

        while (elapsedTime < movementTime)
        {
            float t = elapsedTime / movementTime;
            float currentDiameter = Mathf.Lerp(0f, maxDiameter, t);

            fillImageT.localScale = Vector2.one * aspectRatioSize * currentDiameter;
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    async Task VisualizeFlame()
    {
        if (flameImageT == null)
        {
            Debug.LogError("Flame object is null");
        }

        float elapsedTime = 0f;

        while (elapsedTime < movementTime)
        {
            float t = elapsedTime / movementTime;
            Vector2 pos = Vector2.Lerp(flameStartPos, flameEndPos, t);

            flameImageT.localPosition = pos;
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }
}
