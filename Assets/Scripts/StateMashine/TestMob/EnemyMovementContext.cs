using System;
using UnityEngine;

[Serializable]
public class EnemyMovementContext
{
    [SerializeField]
    Enemy controlledEnemy;

    [SerializeField]
    Rigidbody2D rb;

    Func<GameObject> target; // The closest player

    #region Timers
    public float timerOfCurrentState { get; private set; }

    public enum ERing
    {
        Ring0,
        Ring1,
        Ring2,
        Ring3,
    }

    public ERing GetCurrentRingByPosition()
    {
        float sqrDistanceToTarget = Vector3.SqrMagnitude(
            controlledEnemy.transform.position - target().transform.position
        );
        if (sqrDistanceToTarget > getRing1Radius * getRing1Radius)
            return ERing.Ring0;
        else if (sqrDistanceToTarget > getRing2Radius * getRing2Radius)
            return ERing.Ring1;
        else if (sqrDistanceToTarget > getRing3Radius * getRing3Radius)
            return ERing.Ring2;
        else
            return ERing.Ring3;
    }

    public void UpdateTimers(float deltaTime, ERing currentRing)
    {
        timerOfCurrentState += deltaTime;
        // if (currentRing == ERing.OutsideRing1)
        //     timerOutsideRing1 += deltaTime;
        // else if (currentRing == ERing.OutsideRing2)
        // {
        //     timerOutsideRing1 += deltaTime;
        //     timerOutsideRing2 += deltaTime;
        // }
        // else if (currentRing == ERing.OutsideRing3)
        // {
        //     timerOutsideRing1 += deltaTime;
        //     timerOutsideRing2 += deltaTime;
        //     timerOutsideRing3 += deltaTime;
        // }
        // else if (currentRing == ERing.WithinRing3)
        // {
        //     timerWithinRing3 += deltaTime;
        // }
    }

    public void ResetTimers()
    {
        timerOfCurrentState = 0;
    }
    #endregion

    #region Idle State Variables
    [SerializeField]
    float IdleRotationSpeed;

    [SerializeField]
    float IdleWalkingSpeed;

    [SerializeField, Range(-5f, 5f)]
    float IdleCrossInterference = -1.5f;

    [SerializeField]
    float IdleNoTurningRadius = 3f;

    [SerializeField]
    float IdleMaxRadius = 4f;

    [SerializeField]
    float IdleRadiusRange = 1f;

    [SerializeField]
    float IdleStandingRapidness = 1f;

    [SerializeField, Range(0f, 1f)]
    float IdleStandingThreshold = 0.5f;
    #endregion

    #region Rings
    /// <summary>
    /// Radius to see target
    /// </summary>
    [SerializeField]
    float ring1Radius;

    /// <summary>
    /// Close to enemy but not close enough to attack, where enemy should keep safe distance
    /// </summary>
    [SerializeField]
    float ring2Radius;

    /// <summary>
    /// Close enough to enemy to attack, where enemy should attack (if can)
    /// </summary>
    [SerializeField]
    float ring3Radius;
    #endregion

    #region Time Intervals
    /// <summary>
    /// Minimum time to stay in Idle state
    /// before being able to switch to other state
    /// </summary>
    [SerializeField]
    float minIdleSwitchTime = 2f;

    /// <summary>
    /// Minimum time to stay in LookFor state
    /// before being able to switch to other state
    /// </summary>
    [SerializeField]
    float minLookForSwitchTime = 1f;

    /// <summary>
    /// Minimum time to stay in KeepSafeDistance state
    /// before being able to switch to other state
    /// </summary>
    [SerializeField]
    float minKeepSafeDistanceSwitchTime = 1f;

    /// <summary>
    /// Minimum time to stay in Attack state
    /// before being able to switch to other state
    /// </summary>
    [SerializeField]
    float minAttackSwitchTime = 0.5f;

    /// <summary>
    /// Maximum time to stay in LookFor state
    /// before being forced to switch to Idle state
    /// </summary>
    [SerializeField]
    float maxLookForDuration = 5f;

    /// <summary>
    /// Maximum time to stay in KeepSafeDistance state
    /// before being forced to switch to LookFor state
    /// </summary>
    [SerializeField]
    float maxKeepSafeDistanceDuration = 5f;

    /// <summary>
    /// Maximum time to stay in Attack state
    /// before being forced to switch to KeepSafeDistance state
    /// </summary>
    [SerializeField]
    float maxAttackDuration = 2f;
    #endregion

    public void SetNewTarget(Func<GameObject> newTarget) => target = newTarget;

    public Enemy GetEnemy() => controlledEnemy;

    public Transform GetTransform() => controlledEnemy.transform;

    public GameObject GetTarget() => target();

    public Transform transform => controlledEnemy.transform;
    #region Idle State Getters
    public float getIdleRotationSpeed => IdleRotationSpeed;
    public float getIdleWalkingSpeed => IdleWalkingSpeed;
    public float getIdleCrossInterference => IdleCrossInterference;
    public float getIdleNoTurningRadius => IdleNoTurningRadius;
    public float getIdleMaxRadius => IdleMaxRadius;
    public float getIdleRadiusRange => IdleRadiusRange;
    public float getIdleStandingRapidness => IdleStandingRapidness;
    public float getIdleStandingThreshold => IdleStandingThreshold;
    #endregion

    #region Rings and Times Getters
    public float getRing1Radius => ring1Radius;
    public float getRing2Radius => ring2Radius;
    public float getRing3Radius => ring3Radius;
    public float getMinIdleSwitchTime => minIdleSwitchTime;
    public float getMinLookForSwitchTime => minLookForSwitchTime;
    public float getMinKeepSafeDistanceSwitchTime => minKeepSafeDistanceSwitchTime;
    public float getMinAttackSwitchTime => minAttackSwitchTime;
    public float getMaxLookForDuration => maxLookForDuration;
    public float getMaxKeepSafeDistanceDuration => maxKeepSafeDistanceDuration;
    public float getMaxAttackDuration => maxAttackDuration;
    #endregion

    #region Debug
    public void DebugDrawRings(ERing currentRing)
    {
        Vector2 position = controlledEnemy.transform.position;
        // Draws all rings as circles around position with different colors
        int step = 10;
        for (int i = 0; i < 360; i += step)
        {
            float angle = i * Mathf.Deg2Rad;
            float angleAfterStep = (i + step) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 dirAfterStep = new Vector2(
                Mathf.Cos(angleAfterStep),
                Mathf.Sin(angleAfterStep)
            );

            Debug.DrawLine(
                position + dir * ring1Radius,
                position + dirAfterStep * ring1Radius,
                currentRing == ERing.Ring0 ? Color.yellow : Color.gray
            );

            Debug.DrawLine(
                position + dir * ring2Radius,
                position + dirAfterStep * ring2Radius,
                currentRing == ERing.Ring1 ? Color.magenta : Color.gray
            );

            Debug.DrawLine(
                position + dir * ring3Radius,
                position + dirAfterStep * ring3Radius,
                currentRing == ERing.Ring2 ? Color.red : Color.gray
            );
        }
    }
    #endregion

    public Rigidbody2D GetRigidbody2D() => rb;

    public EnemyMovementContext(Enemy controlledEnemy)
    {
        this.controlledEnemy = controlledEnemy;
    }
}
