using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private AIMovementComponent movement;
    private IChangeState changeState;
    private IAIBehaviour behaviour;

    public EnemyChaseState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        movement = context.movement;
        changeState = context.changeState;
        behaviour = context.behaviour;
    }
    public void Enter()
    {
        movement.UseCombatIdle();
    }

    public void Exit()
    {
        movement.Stop();
    }

    public void Update()
    {
        Move();
        CheckTarget();
    }

    private void Move()
    {
        movement.MoveTo(detection.GetTargetPosition());
    }

    private void CheckTarget()
    {
        if (detection.PlayerIsInActionDistance())
            changeState.ChangeState(behaviour.OnDestination());
    }
}
