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
        if (ScoreManager.instance != null) ScoreManager.instance.scoreKill();
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
