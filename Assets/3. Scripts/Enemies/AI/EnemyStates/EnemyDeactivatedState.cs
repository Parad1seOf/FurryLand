using UnityEngine;

public class EnemyDeactivatedState : IEnemyState
{
    private AIContext context;

    public EnemyDeactivatedState(AIContext context)
    {
        this.context = context;
    }

    public void Enter()
    {
        context.gameObject.SetActive(false);
    }

    public void Exit()
    {
        context.gameObject.SetActive(true);
    }

    public void Update()
    {
        
    }
}
