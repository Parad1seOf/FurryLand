using UnityEngine;

public class ElephantBehaviour : MonoBehaviour, IAIBehaviour
{
    [SerializeField] private AIContext context;

    public void Awake()
    {
        if (context == null)
            context = GetComponent<AIContext>();
    }

    public IEnemyState AfterAlert()
    {
        throw new System.NotImplementedException();
    }

    public IEnemyState OnAlert()
    {
        throw new System.NotImplementedException();
    }

    public IEnemyState OnDestination()
    {
        return new EnemyWatergunState(context);
    }

    public IEnemyState OnDie()
    {
        return new EnemyDeadState(context);
    }

    public IEnemyState OnPlayerUntargetable()
    {
        throw new System.NotImplementedException();
    }

    public IEnemyState OnRespawn()
    {
        return new EnemyPathingState(context);
    }

    public IEnemyState OnStart()
    {
        return new EnemyDeadState(context);
    }

    public IEnemyState OnSuspicion()
    {
        throw new System.NotImplementedException();
    }
}
