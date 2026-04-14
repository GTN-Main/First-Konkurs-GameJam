using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Paintball : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;

    [SerializeField]
    private int explosionDamage = 50;

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
    private Transform mainImageT;

    [SerializeField]
    private float maxHeight = 3f;

    [SerializeField]
    AnimationCurve heightOverTimeCurve;

    Rigidbody2D rb;

    public void Init(
        Vector2 position,
        float explosionRadius,
        int explosionDamage,
        Vector2 movementVector,
        float movementDistance,
        float movementTime,
        LayerMask damageableLayers
    )
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = position;
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
        VisualizeHeight();
    }

    IEnumerator MoveRoutine()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + movementVector.normalized * movementDistance;
        Debug.Log(
            $"Moving paintball from {startPosition} to {targetPosition} over {movementTime} seconds."
        );

        rb.WakeUp();

        while (elapsedTime < movementTime)
        {
            elapsedTime += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / movementTime);
            float curveT = moveOverTimeCurve.Evaluate(t);

            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, curveT);

            rb.MovePosition(newPosition);
            //Debug.Log($"Moving paintball to {newPosition} with velocity {rb.linearVelocity}");

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        Explode();
    }

    async Task Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            damageableLayers
        );
        foreach (Collider2D hitCollider in hitColliders)
        {
            PlayerController pC = hitCollider.GetComponent<PlayerController>();
            if (pC == null)
                continue;
            PlayerTag playerTag = pC.GetPlayerTag();
            HealthManager.Instance.DamagePlayer(playerTag, explosionDamage);
        }

        MyAudioEffects.Instance.DoEffect("PaintExplosion", transform.position, 1f);

        Destroy(gameObject);
    }

    async Task VisualizeExplosionRadius()
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + movementVector.normalized * movementDistance;
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
            maxRadiusImageT.transform.position = targetPosition;
            fillImageT.transform.position = targetPosition;
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    async Task VisualizeHeight()
    {
        if (mainImageT.gameObject == null)
        {
            Debug.LogError("Main image object is null");
        }

        float elapsedTime = 0f;

        while (elapsedTime < movementTime)
        {
            float t = elapsedTime / movementTime;
            float currentHeight = heightOverTimeCurve.Evaluate(t) * maxHeight;

            mainImageT.localPosition = new Vector2(0, currentHeight);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }
}
