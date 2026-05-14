using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    AIContext context;
    private AIPathingComponent pathing;
    private IAIBehaviour behaviour;
    private AIMovementComponent movement;
    private IChangeState changeState;
    private DetectionComponent detection;

    private Vector3 nextPoint;

    public EnemyPatrolState(AIContext context)
    {
        this.context = context;
        pathing = context.pathing;
        behaviour = context.behaviour;
        movement = context.movement;
        changeState = context.changeState;
        detection = context.detection;
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
        LookForSuspiciousActivity();
        if (!pathing.HasArrived()) return;

        nextPoint = pathing.GetNextPoint();
        movement.MoveTo(nextPoint);
    }

    private void LookForSuspiciousActivity()
    {
        if (detection.SeesSuspiciousConduct())
            changeState.ChangeState(behaviour.OnSuspicion());

        if (detection.PlayerIsTooClose())
            changeState.ChangeState(behaviour.OnAlert());
    }
}
