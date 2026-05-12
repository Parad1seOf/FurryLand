using UnityEngine;

public class EnemySearchingState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private AIMovementComponent movement;
    private IChangeState changeState;
    private IAIBehaviour behaviour;

    public EnemySearchingState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        movement = context.movement;
        changeState = context.changeState;
        behaviour = context.behaviour;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        context.attack.EndAttack();
        movement.LookAt(detection.GetTargetPosition());
        if (detection.PlayerIsTooClose()) return;
        if (detection.PlayerIsInActionDistance() && detection.SeesPlayer())
        {
            changeState.ChangeState(behaviour.OnDestination());
            
        }
    }
}
