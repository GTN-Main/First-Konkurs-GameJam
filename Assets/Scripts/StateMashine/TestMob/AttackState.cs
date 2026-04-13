using UnityEngine;

public class AttackState : EnemyMovementState
{
    GameObject target;
    bool mustBeCloser = false;
    float attackCooldown = 0f;

    public AttackState(EnemyMovementContext context, EnemyMovementStateMachine.EEnemyState stateKey)
        : base(context, stateKey) { }

    public override void EnterState()
    {
        context.ResetTimers();
        mustBeCloser = false;
    }

    public override void UpdateState()
    {
        if ((target = context.GetTarget()) == null)
            return;

        float sqrMDistance = Vector2.SqrMagnitude(
            (Vector2)context.GetTransform().position - (Vector2)target.transform.position
        );
        Vector2 dirForward = (
            (Vector2)target.transform.position - (Vector2)context.GetTransform().position
        ).normalized;
        float maxAttackDistance = context.getRing3Radius * context.getRing3Radius;

        if (sqrMDistance > maxAttackDistance)
            mustBeCloser = true;
        else if (sqrMDistance < maxAttackDistance * 0.9f)
            mustBeCloser = false;

        if (mustBeCloser)
            // Attack (move towards the target)
            context
                .GetEnemy()
                .Move(
                    dirForward * context.getIdleWalkingSpeed * Time.deltaTime
                );
        else
        {
            if (attackCooldown <= 0f)
                Attack();
            else
                attackCooldown -= Time.deltaTime;
        }

        /*
        // Retreat (pull out)
        context
            .GetCharacterController()
            .Move(
                -dirForward * context.getIdleWalkingSpeed * Time.deltaTime
                    + Vector3.down * Time.deltaTime
            );
        */

        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();
        context.DebugDrawRings(currentRing);
        context.UpdateTimers(Time.deltaTime, currentRing);
    }

    void Attack()
    {
        context.GetEnemy().MakeAttack(context.GetTarget());
        (bool hasWeapon, float cooldown) = context.GetEnemy().GetAttackCooldown();
        if (hasWeapon)
            attackCooldown = cooldown;
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Attack State");
    }

    public override EnemyMovementStateMachine.EEnemyState GetNextState()
    {
        EnemyMovementContext.ERing currentRing = context.GetCurrentRingByPosition();

        if (
            currentRing != EnemyMovementContext.ERing.Ring3
                && context.timerOfCurrentState >= context.getMinAttackSwitchTime
            || currentRing == EnemyMovementContext.ERing.Ring3
                && context.timerOfCurrentState >= context.getMaxAttackDuration
        )
        {
            return EnemyMovementStateMachine.EEnemyState.KeepSafeDistanceState;
        }

        return stateKey;
    }
}
