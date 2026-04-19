using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : IEnemyState
{
    private AIContext          context;
    private DetectionComponent detection;
    private SuspicionComponent suspicion;
    private IChangeState       changeState;
    private NavMeshAgent       agent;
    private Transform[]        waypoints;

    private int   currentWaypoint    = 0;
    private float waitTimer          = 0f;
    private const float WaitTime          = 2f;
    private const float ArrivalThreshold  = 0.5f;

    public EnemyPatrolState(AIContext context)
    {
        this.context     = context;
        this.detection   = context.detection;
        this.suspicion   = context.suspicion;
        this.changeState = context.changeState;
        this.agent       = context.agent;
        this.waypoints   = context.waypoints;
    }

    public void Enter()
    {
        GoToCurrentWaypoint();
    }

    public void Exit()
    {
        if (agent != null) agent.ResetPath();
    }

    public void Update()
    {
        detection.TickSuspicion(suspicion);

        if (suspicion.IsSuspicious())
        {
            changeState.ChangeState(new EnemySuspicionState(context));
            return;
        }

        if (agent == null || waypoints == null || waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < ArrivalThreshold)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= WaitTime)
            {
                waitTimer = 0f;
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                GoToCurrentWaypoint();
            }
        }
    }

    private void GoToCurrentWaypoint()
    {
        if (agent == null || waypoints == null || waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
    }
}