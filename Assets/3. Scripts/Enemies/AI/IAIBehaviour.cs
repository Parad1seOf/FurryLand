using UnityEngine;

public interface IAIBehaviour
{
    public IEnemyState OnStart();
    public IEnemyState OnSuspicion();
    public IEnemyState OnAlert();
    public IEnemyState AfterAlert();
    public IEnemyState OnDestination();
    public IEnemyState OnPlayerUntargetable();
    public IEnemyState OnRespawn();
    public IEnemyState OnDie();
}
