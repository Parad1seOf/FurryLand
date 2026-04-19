using UnityEngine;

public class EnemySuspicionState : IEnemyState
{
    private AIContext          context;
    private DetectionComponent detection;
    private SuspicionComponent suspicion;
    private IChangeState       changeState;

    private float minTimer;
    private const float MinStateDuration = 2f;

    public EnemySuspicionState(AIContext context)
    {
        this.context     = context;
        this.detection   = context.detection;
        this.suspicion   = context.suspicion;
        this.changeState = context.changeState;
    }

    public void Enter()
    {
        minTimer = MinStateDuration;
        if (context.agent != null) context.agent.ResetPath();
    }

    public void Exit() { }

    public void Update()
    {
        minTimer -= Time.deltaTime;

        detection.TickSuspicion(suspicion);

        // No puede volver a Idle hasta que pasen los 2 segundos mínimos
        if (minTimer <= 0f && !suspicion.IsSuspicious())
            changeState.ChangeState(new EnemyIdleState(context));
    }
}