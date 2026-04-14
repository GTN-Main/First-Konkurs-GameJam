using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float smoothing = 5f;
    public float lookAhead = 2f;
    public float minSize = 5f;
    public float padding = 2f;

    private Camera _cam;
    private Rigidbody2D _rb1,
        _rb2;
    private Transform p1 => _rb1.transform;
    private Transform p2 => _rb2.transform;

    private bool canRun = false;

    public void Init()
    {
        _cam = GetComponent<Camera>();
        _rb1 = PlayerSpawnManager.Instance?.Player1?.GetComponent<Rigidbody2D>();
        _rb2 = PlayerSpawnManager.Instance?.Player2?.GetComponent<Rigidbody2D>();
        canRun = _rb1 != null || _rb2 != null;
    }

    void LateUpdate()
    {
        if (!canRun)
            return;

        Vector2 v1 = _rb1.linearVelocity;
        Vector2 v2 = _rb2.linearVelocity;

        float targetX =
            ((p1.position.x + p2.position.x) * 0.5f) + ((v1.x + v2.x) * 0.5f * lookAhead);
        float targetY =
            ((p1.position.y + p2.position.y) * 0.5f) + ((v1.y + v2.y) * 0.5f * lookAhead);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(targetX, targetY, -10f),
            smoothing * Time.deltaTime
        );

        float differenceX = p1.position.x - p2.position.x;
        if (differenceX < 0)
            differenceX = -differenceX;

        float differenceY = p1.position.y - p2.position.y;
        if (differenceY < 0)
            differenceY = -differenceY;

        float camSizeX = differenceX * 0.5f / _cam.aspect;
        float camSizeY = differenceY * 0.5f;

        float targetSize = (camSizeX > camSizeY ? camSizeX : camSizeY) + padding;

        _cam.orthographicSize = Mathf.Lerp(
            _cam.orthographicSize,
            targetSize < minSize ? minSize : targetSize,
            smoothing * Time.deltaTime
        );
    }
}
