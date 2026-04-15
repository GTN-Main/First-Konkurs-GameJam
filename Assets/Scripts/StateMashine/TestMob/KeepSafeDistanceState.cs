using UnityEngine;

public class KeepSafeDistanceState : EnemyMovementState
{
    GameObject target;
    float rightPositioning;
    bool isRetreating = true;

    public KeepSafeDistanceState(
        EnemyMovementContext context,
        EnemyMovementStateMachine.EEnemyState stateKey
    )
        : base(context, stateKey) { }

    public override void EnterState()
    {
        context.ResetTimers();
        isRetreating = true;

        // Generate random value for right positioning (moving sideways) to make the movement less predictable
        rightPositioning = 0f;
        rightPositioning = Mathf.PerlinNoise1D(Random.Range(0f, 100f)) * 5f - 2.5f;
        rightPositioning =
            Mathf.Abs(rightPositioning) < 0.5f
                ? rightPositioning < 0
                    ? -1f
                    : 1f
                : rightPositioning;
        //Debug.Log($"Generated rightPositioning value: {rightPositioning}");

        //Debug.Log("Entering KeepSafeDistance State");
    }

    public override void UpdateState()
    {
        if ((target = context.GetTarget()) == null)
            return;

        float sqrMDistance = Vector3.SqrMagnitude(
            context.GetTransform().position - target.transform.position
        );
        Vector2 dirForward = (
            target.transform.position - context.GetTransform().position
        ).normalized;
        Vector2 dirRight = Vector3.Cross(dirForward, Vector3.back).normalized;

        float maxsafeDistance = context.getRing2Radius * context.getRing2Radius;
        float minsafeDistance = context.getRing3Radius * context.getRing3Radius;

        if (sqrMDistance > maxsafeDistance)
        {
            isRetreating = false;
        }
        else if (sqrMDistance < minsafeDistance)
        {
            isRetreating = true;
        }

        if (isRetreating)
            // Retreat (pull out)
            context
                .GetEnemy()
                .Move(
                    (-dirForward + rightPositioning * dirRight).normalized
                        * context.getIdleWalkingSpeed
                        * Time.fixedDeltaTime
                );
        else
            // Approach (pull in)
            context
                .GetEnemy()
                .Move(
                    (dirForward + rightPositioning * dirRight).normalized
                        * context.getIdleWalkingSpeed
                        * Time.fixedDeltaTime
                );

        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();
        context.DebugDrawRings(currentRing);
        context.UpdateTimers(Time.fixedDeltaTime, currentRing);
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting KeepSafeDistance State");
    }

    public override EnemyMovementStateMachine.EEnemyState GetNextState()
    {
        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();

        if (
            (
                currentRing == EnemyMovementContext.ERing.Ring0
                || currentRing == EnemyMovementContext.ERing.Ring1
            )
            && context.timerOfCurrentState >= context.getMinKeepSafeDistanceSwitchTime
        )
        {
            return EnemyMovementStateMachine.EEnemyState.LookForTargetState;
        }

        if (
            currentRing == EnemyMovementContext.ERing.Ring3
                && context.timerOfCurrentState >= context.getMinKeepSafeDistanceSwitchTime
            || currentRing == EnemyMovementContext.ERing.Ring2
                && context.timerOfCurrentState >= context.getMaxKeepSafeDistanceDuration
        )
        {
            return EnemyMovementStateMachine.EEnemyState.AttackState;
        }
        return stateKey;
    }
}
