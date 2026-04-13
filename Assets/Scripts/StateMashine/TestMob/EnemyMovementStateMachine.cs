using UnityEngine;

public class EnemyMovementStateMachine : StateManager<EnemyMovementStateMachine.EEnemyState>
{
    public enum EEnemyState
    {
        IdleState,
        LookForTargetState,
        KeepSafeDistanceState,
        AttackState,
    }

    [SerializeField]
    EnemyMovementContext context;

    /*
        <All logic>

        Functions:
        Idle
            just walk around in circles

        LookForTarget
            go in direction to be closer to target
                
        KeepSafeDistance
            walk around the target and if attack
            conditions are met proceed to attack

        Attack
            wait for opportunity and attack target
            but ensure not to be too close to target for too long

        Conditions to switch states:
            outside of ring1
             + time outside ring1 threshold
              -> Idle

            within ring1 and outside of ring2
            -> LookForTarget

            within ring2 and outside of ring3 or
            (if privious attack) max Time threshold
             -> KeepSafeDistance
            
            within ring3 or
            (if previous keep safe distance) max Time threshold
             -> Attack
    */

    void Awake()
    {
        context.SetNewTarget(() => PlayersPosManager.Instance.GetClosestPlayerToPoint(context.GetTransform().position));
        InitializeStates();
    }

    void InitializeStates()
    {
        States.Add(EEnemyState.IdleState, new IdleState(context, EEnemyState.IdleState));
        States.Add(
            EEnemyState.LookForTargetState,
            new LookForTargetState(context, EEnemyState.LookForTargetState)
        );
        States.Add(
            EEnemyState.KeepSafeDistanceState,
            new KeepSafeDistanceState(context, EEnemyState.KeepSafeDistanceState)
        );
        States.Add(EEnemyState.AttackState, new AttackState(context, EEnemyState.AttackState));
        currentState = States[EEnemyState.IdleState];
    }
}
