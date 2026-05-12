using UnityEngine;

public class SlothAttackState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private EnemyAttack attack;
    private IChangeState changeState;
    private AIMovementComponent movement;

    public SlothAttackState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        attack = context.attack;
        changeState = context.changeState;
        movement = context.movement;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        attack.EndAttack();
    }

    public void Update()
    {
        movement.LookAt(detection.GetTargetPosition());
        if (detection.PlayerIsTooClose() || detection.PlayerEscapedAttack())
        {
            changeState.ChangeState(changeState.PreviousState());
        }

        attack.Attack(detection.GetTargetPosition());
    }
}
