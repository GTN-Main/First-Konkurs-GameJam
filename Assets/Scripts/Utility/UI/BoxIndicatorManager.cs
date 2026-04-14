using System;
using UnityEngine;

public class BoxIndicatorManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 targetPosition;

    [SerializeField]
    private bool hasTargetPosition = false;

    [SerializeField]
    private RectTransform indicatorRectTransform;

    [SerializeField]
    private float timerToFindTargetBox = 0f;

    [SerializeField]
    private float timeToFindTargetBox_Cooldown = 1f;

    [SerializeField]
    private float marginFromScreenEdge = 100f;

    [SerializeField]
    private float offsetAngle = 0f;

    [SerializeField]
    private Camera cameraUI;

    [SerializeField]
    private Canvas canvas;

    void OnEnable()
    {
        indicatorRectTransform.gameObject.SetActive(false);
        AssignCamera();
    }

    void AssignCamera()
    {
        if (canvas.worldCamera == null && GameManager.Instance != null)
        {
            cameraUI = GameManager.Instance.GetCamera();
            canvas.worldCamera = cameraUI;
        }
    }

    void UpdateTargetBoxPosition()
    {
        timerToFindTargetBox += Time.deltaTime;
        if (timerToFindTargetBox >= timeToFindTargetBox_Cooldown)
        {
            Vector3? targetPosition =
                BoxesSpawnManager.Instance?.GetClosestBoxPositionToPoint(
                    Camera.main.transform.position
                ) ?? null;
            hasTargetPosition = targetPosition.HasValue;
            if (targetPosition.HasValue)
            {
                this.targetPosition = targetPosition.Value;
            }
            else
            {
                Debug.Log(
                    $"Is BoxIndicatorManager null? {BoxesSpawnManager.Instance == null} Is Camera null? {Camera.main == null} "
                );
            }
            timerToFindTargetBox = 0f;
        }
    }

    void Update()
    {
        if (cameraUI == null)
            return;

        if (
            GameManager.Instance == null
            || GameManager.Instance.GetCurrentGameStateTag() != GameManager.GameStateTag.StartGame
        )
        {
            indicatorRectTransform.gameObject.SetActive(false);
            return;
        }

        UpdateTargetBoxPosition();

        if (hasTargetPosition)
        {
            if (IsBoxOffScreen(targetPosition, out Vector3 targetPositionOnScreen))
            {
                Vector3 clampedTargetPositionOnScreen = ClampToScreen(
                    targetPositionOnScreen,
                    marginFromScreenEdge
                );
                Vector3 indicatorWorldPosition = GetWorldPositionFromScreen(
                    clampedTargetPositionOnScreen
                );
                indicatorRectTransform.position = Vector3.Scale(
                    indicatorWorldPosition,
                    new Vector3(1, 1, 0)
                );
                float angle =
                    GetAngleToTarget(cameraUI.transform.position, indicatorWorldPosition)
                    + offsetAngle;
                indicatorRectTransform.localEulerAngles = new Vector3(0, 0, angle);
                indicatorRectTransform.gameObject.SetActive(true);
            }
            else
            {
                indicatorRectTransform.gameObject.SetActive(false);
            }
        }
    }

    bool IsBoxOffScreen(Vector3 worldPosition, out Vector3 screenPosition)
    {
        screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        bool isOffScreen =
            screenPosition.x <= marginFromScreenEdge
            || screenPosition.x >= Screen.width - marginFromScreenEdge
            || screenPosition.y <= marginFromScreenEdge
            || screenPosition.y >= Screen.height - marginFromScreenEdge;
        return isOffScreen;
    }

    Vector3 ClampToScreen(Vector3 screenPosition, float margin)
    {
        float clampedX = Mathf.Clamp(screenPosition.x, margin, Screen.width - margin);
        float clampedY = Mathf.Clamp(screenPosition.y, margin, Screen.height - margin);
        return new Vector3(clampedX, clampedY, 0);
    }

    Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        return cameraUI.ScreenToWorldPoint(screenPosition);
    }

    float GetAngleToTarget(Vector3 obj1, Vector3 obj2)
    {
        Vector3 dir = (obj2 - obj1).normalized;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg % 360;
    }
}
