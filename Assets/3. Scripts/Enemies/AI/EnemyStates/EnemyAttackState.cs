using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private EnemyAttack attack;
    private IChangeState changeState;

    public EnemyAttackState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        attack = context.attack;
        changeState = context.changeState;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        Rotate();
        if (attack.isAttacking) return; 
        CheckTarget();
        Attack();
    }

    void CheckTarget()
    {
        if (detection.PlayerEscapedAttack())
        {
            changeState.ChangeState(changeState.PreviousState());
        }
    }

    private void Attack()
    {
        attack.Attack(detection.GetTargetDirection());
    }

    private void Rotate()
    {
        context.movement.LookAt(detection.GetTargetDirection());
    }
}
