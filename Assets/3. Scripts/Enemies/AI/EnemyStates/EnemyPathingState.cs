using UnityEngine;

public class EnemyPathingState : IEnemyState
{
    private AIContext context;
    private AIPathingComponent pathing;
    private IAIBehaviour behaviour;
    private AIMovementComponent movement;
    private IChangeState changeState;

    private Vector3 nextPoint;

    public EnemyPathingState(AIContext context)
    {
        this.context = context;
        pathing = context.pathing;
        behaviour = context.behaviour;
        movement = context.movement;
        changeState = context.changeState;
        
    }

    public void Enter()
    {
        nextPoint = pathing.GetClosestPoint();
        movement.MoveTo(nextPoint);
    }

    public void Exit()
    {
        movement.Stop();
    }

    public void Update()
    {
        if (!pathing.HasArrived()) return;

        nextPoint = pathing.GetNextPoint();
        if (nextPoint == Vector3.zero)
        {
            changeState.ChangeState(behaviour.OnDestination());
            return;
        }

        movement.MoveTo(nextPoint);
    }
}
