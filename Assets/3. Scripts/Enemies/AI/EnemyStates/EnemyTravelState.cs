using UnityEngine;

public class EnemyTravelState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private AIMovementComponent movement;
    private IChangeState changeState;

    public EnemyTravelState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        movement = context.movement;
        changeState = context.changeState;
    }
    public void Enter()
    {
        
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
            changeState.ChangeState(new EnemyAttackState(context));
    }
}
