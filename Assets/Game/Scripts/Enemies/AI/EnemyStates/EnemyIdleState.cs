using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private AIContext          context;
    private DetectionComponent detection;
    private SuspicionComponent suspicion;
    private IChangeState       changeState;

    public EnemyIdleState(AIContext context)
    {
        this.context     = context;
        this.detection   = context.detection;
        this.suspicion   = context.suspicion;
        this.changeState = context.changeState;
    }

    public void Enter()
    {
        suspicion.ResetSuspicionLevel();
        if (context.agent != null) context.agent.ResetPath();
    }

    public void Exit() { }

    public void Update()
    {
        detection.TickSuspicion(suspicion);

        if (suspicion.IsSuspicious())
            changeState.ChangeState(new EnemySuspicionState(context));
    }
}