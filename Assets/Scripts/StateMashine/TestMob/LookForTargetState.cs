using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LookForTargetState : EnemyMovementState
{
    HexagonalGridPathFinding.PathResultData pathResult = null;
    Vector2 dirForward = Vector2.zero;
    Vector2? nextPoint = null;
    float findNewRouteTimer = 0f;
    float findNewRouteInterval = 0.4f;

    public LookForTargetState(
        EnemyMovementContext context,
        EnemyMovementStateMachine.EEnemyState stateKey
    )
        : base(context, stateKey) { }

    public override void EnterState()
    {
        //Debug.Log("Entering LookForTarget State");
        pathResult = null;
        nextPoint = null;
        context.ResetTimers();
        WaitForPath();
    }

    async Task WaitForPath()
    {
        if (context.GetTarget() == null)
            return;

        pathResult = await HexagonalGridPathFinding.Instance.FindPathAsync(
            (Vector2)context.transform.position,
            (Vector2)context.GetTarget().transform.position
        );
    }

    public override void UpdateState()
    {
        //Debug.Log("Updating LookForTarget State");
        Debug.DrawLine(
            (Vector2)context.transform.position,
            (Vector2)context.GetTarget().transform.position,
            Color.gold
        );

        if (findNewRouteTimer >= findNewRouteInterval)
        {
            findNewRouteTimer = 0f;
            WaitForPath();
        }
        findNewRouteTimer += Time.deltaTime;

        if (
            nextPoint == null
            && pathResult != null
            && pathResult.foundPath
            && pathResult.path.Count > 0
        )
        {
            nextPoint = pathResult.path[0].coordinates;
        }
        try
        {
            if (nextPoint != null)
            {
                if (Mathf.Abs(((Vector2)context.transform.position - nextPoint.Value).sqrMagnitude) < 0.2f)
                {
                    if (pathResult.path.Count > 0)
                    {
                        pathResult.path.RemoveAt(0);
                        HexagonalGridPathFinding.HexCell nextHexCell =
                            pathResult.path.FirstOrDefault();
                        if (nextHexCell != null)
                            nextPoint = nextHexCell.coordinates;
                        else
                            nextPoint = null;
                    }
                }

                dirForward = (nextPoint.Value - (Vector2)context.transform.position).normalized;
                context
                    .GetEnemy()
                    .Move(
                        dirForward * context.getIdleWalkingSpeed * Time.deltaTime
                    );
            }
        }
        catch (System.Exception ex)
        {
            DebugUtility.WriteInColor($"Exception in LookForTargetState: {ex.Message}", Color.red);
        }

        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();
        context.DebugDrawRings(currentRing);
        context.UpdateTimers(Time.deltaTime, currentRing);
    }

    public override void ExitState()
    {
        //Debug.Log("Exiting LookForTarget State");
    }

    public override EnemyMovementStateMachine.EEnemyState GetNextState()
    {
        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();

        if (
            currentRing == EnemyMovementContext.ERing.Ring0
                && context.timerOfCurrentState >= context.getMinLookForSwitchTime
            || currentRing == EnemyMovementContext.ERing.Ring1
                && context.timerOfCurrentState >= context.getMaxLookForDuration
        )
        {
            return EnemyMovementStateMachine.EEnemyState.IdleState;
        }

        if (
            (
                currentRing == EnemyMovementContext.ERing.Ring2
                || currentRing == EnemyMovementContext.ERing.Ring3
            )
            && context.timerOfCurrentState >= context.getMinLookForSwitchTime
        )
        {
            return EnemyMovementStateMachine.EEnemyState.KeepSafeDistanceState;
        }

        return stateKey;
    }
}
