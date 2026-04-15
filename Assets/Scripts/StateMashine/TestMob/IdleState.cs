using UnityEngine;

public class IdleState : EnemyMovementState
{
    Vector2 idleTarget = Vector2.zero;
    Vector2 dirForward = Vector2.zero;

    bool isTurningAround = false;
    float acceleration = 0f;

    float currentCrossInterference = 0f;
    float currentRadiusOffset = 0f;

    public IdleState(EnemyMovementContext context, EnemyMovementStateMachine.EEnemyState stateKey)
        : base(context, stateKey) { }

    public override void EnterState()
    {
        //Debug.Log("Entering Idle State");
        context.ResetTimers();
        if (idleTarget == Vector2.zero)
        {
            idleTarget =
                (Vector2)UnityEngine.Random.insideUnitCircle * 2f
                + (Vector2)context.transform.position;
        }
        dirForward = (idleTarget - (Vector2)context.transform.position).normalized;
        isTurningAround = false;
        acceleration = 0f;
        currentRadiusOffset = UnityEngine.Random.Range(-context.getIdleRadiusRange, 0);
    }

    public override void UpdateState()
    {
        //Debug.Log($"Updating Idle State\nidleTarget: {idleTarget}");

        Debug.DrawLine(context.transform.position, idleTarget, Color.gold);

        float sqrMFromIdleTarget = Vector2.SqrMagnitude(
            (Vector2)context.transform.position - idleTarget
        );
        bool shouldTurnAround =
            sqrMFromIdleTarget
            > (context.getIdleMaxRadius + currentRadiusOffset)
                * (context.getIdleMaxRadius + currentRadiusOffset);
        bool shouldStopTurnAround =
            sqrMFromIdleTarget
            < (context.getIdleNoTurningRadius + currentRadiusOffset)
                * (context.getIdleNoTurningRadius + currentRadiusOffset);

        if (!isTurningAround && shouldTurnAround)
        {
            isTurningAround = true;
            currentCrossInterference =
                Mathf.PerlinNoise1D(context.timerOfCurrentState * 0.5f) < 0.5f
                    ? -context.getIdleCrossInterference
                    : context.getIdleCrossInterference;
        }
        else if (isTurningAround && shouldStopTurnAround)
        {
            isTurningAround = false;
            currentRadiusOffset = UnityEngine.Random.Range(-context.getIdleRadiusRange, 0);
        }

        Vector2 vNext = isTurningAround
            ? (idleTarget - (Vector2)context.transform.position)
            : dirForward;
        if (isTurningAround)
            vNext += (Vector2)Vector3.Cross(Vector3.back, vNext) * currentCrossInterference;
        vNext.Normalize();

        if (
            Mathf.PerlinNoise1D(context.timerOfCurrentState * context.getIdleStandingRapidness)
            < context.getIdleStandingThreshold
        )
            acceleration = Mathf.MoveTowards(acceleration, 0f, Time.fixedDeltaTime);
        else
            acceleration = Mathf.MoveTowards(acceleration, 1f, Time.fixedDeltaTime);

        dirForward = Vector3.RotateTowards(
            dirForward,
            vNext,
            context.getIdleRotationSpeed * acceleration * Time.fixedDeltaTime,
            0f
        );
        context
            .GetEnemy()
            .Move(acceleration * dirForward * context.getIdleWalkingSpeed * Time.fixedDeltaTime);

        Debug.DrawRay(context.transform.position, dirForward, Color.cyan);

        Debug.DrawRay(context.transform.position, vNext, Color.magenta);

        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();
        context.DebugDrawRings(currentRing);
        context.UpdateTimers(Time.fixedDeltaTime, currentRing);
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting Idle State");
    }

    public override EnemyMovementStateMachine.EEnemyState GetNextState()
    {
        if (
            context.GetCurrentRingByPosition() != EnemyMovementContext.ERing.Ring0
            && context.timerOfCurrentState >= context.getMinIdleSwitchTime
        )
        {
            return EnemyMovementStateMachine.EEnemyState.LookForTargetState;
        }

        return stateKey;
    }
}
