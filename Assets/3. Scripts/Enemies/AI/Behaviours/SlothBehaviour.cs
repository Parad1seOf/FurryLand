using UnityEngine;

public class SlothBehaviour : MonoBehaviour, IAIBehaviour
{
    private AIContext context;

    public void Start()
    {
        context = GetComponent<AIContext>();
    }

    public IEnemyState AfterAlert()
    {
        return new EnemySearchingState(context);
    }

    public IEnemyState OnAlert()
    {
        return new EnemyAlertState(context);
    }

    public IEnemyState OnDestination()
    {
        return new SlothAttackState(context);
    }

    public IEnemyState OnDie()
    {
        return new EnemyDeadState(context);
    }

    public IEnemyState OnPlayerUntargetable()
    {
        return new EnemySearchingState(context);
    }

    public IEnemyState OnRespawn()
    {
        return new EnemySearchingState(context);
    }

    public IEnemyState OnStart()
    {
        return new EnemyIdleState(context);
    }

    public IEnemyState OnSuspicion()
    {
        return new EnemySuspicionState(context);
    }
}
