using UnityEngine;

public class RabbitBehaviour : MonoBehaviour, IAIBehaviour
{
    [SerializeField] private AIContext context;

    public void Start()
    {
        if (context == null)
            context = GetComponent<AIContext>();
    }

    public IEnemyState AfterAlert()
    {
        return new EnemyChaseState(context);
    }

    public IEnemyState OnAlert()
    {
        return new EnemyAlertState(context);
    }

    public IEnemyState OnDestination()
    {
        return new EnemyAttackState(context);
    }

    public IEnemyState OnPlayerUntargetable()
    {
        return new EnemyChaseState(context);
    }

    public IEnemyState OnStart()
    {
        if (context.pathing != null) return new EnemyPatrolState(context);
        return new EnemyIdleState(context);
    }

    public IEnemyState OnSuspicion()
    {
        return new EnemySuspicionState(context);
    }

    public IEnemyState OnRespawn()
    {
        return new EnemyChaseState(context);
    }

    public IEnemyState OnDie()
    {
        return new EnemyDeadState(context);
    }
}
