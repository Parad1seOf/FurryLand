using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    AIContext context;

    public EnemyDeadState(AIContext context)
    {
        this.context = context;

    }

    public void Enter()
    {
        context.detection.gameObject.SetActive(false);
    }

    public void Exit()
    {
        context.detection.gameObject.SetActive(true);
    }

    public void Update()
    {
        
    }
}
