using UnityEngine;

public class EnemyTravelState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private AIMovementComponent movement;

    public EnemyTravelState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        movement = context.movement;
    }
    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        movement.MoveTo(detection.GetTargetPosition());
    }

    
}
