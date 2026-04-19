using UnityEngine;
using UnityEngine.AI;

public class EnemyAlertState : IEnemyState
{
    private AIContext    context;
    private NavMeshAgent agent;
    private Transform    player;

    public EnemyAlertState(AIContext context)
    {
        this.context = context;
        this.agent   = context.agent;
        this.player  = context.playerTransform;
    }

    public void Enter()
    {
        Debug.Log("[EnemyAlertState] Enemigo en ALERTA.");
    }

    public void Exit()
    {
        if (agent != null) agent.ResetPath();
    }

    public void Update()
    {
        if (agent == null || player == null) return;
        agent.SetDestination(player.position);
    }
}