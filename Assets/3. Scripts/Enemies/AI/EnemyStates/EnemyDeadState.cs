using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    private AIContext context;

    public EnemyDeadState(AIContext context)
    {
        this.context = context;
    }

    public void Enter()
    {
        if (ScoreManager.instance != null)
            ScoreManager.instance.scoreKill();

        RagdollController ragdoll = context.gameObject.GetComponent<RagdollController>();

        if (ragdoll == null)
        {
            context.gameObject.SetActive(false);
        }
    }

    public void Exit()
    {
        context.gameObject.SetActive(true);
    }

    public void Update()
    {
    }
}