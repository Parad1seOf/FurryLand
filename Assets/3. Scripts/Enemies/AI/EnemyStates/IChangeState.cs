using UnityEngine;

public interface IChangeState
{
    public void ChangeState(IEnemyState newState);
    public IEnemyState PreviousState();
}
