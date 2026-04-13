public abstract class EnemyMovementState : BaseState<EnemyMovementStateMachine.EEnemyState>
{
    protected EnemyMovementContext context;

    public EnemyMovementState(
        EnemyMovementContext context,
        EnemyMovementStateMachine.EEnemyState stateKey
    )
        : base(stateKey)
    {
        this.context = context;
    }
}
