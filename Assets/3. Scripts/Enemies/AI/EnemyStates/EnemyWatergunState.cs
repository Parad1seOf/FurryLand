using UnityEngine;

public class EnemyWatergunState : IEnemyState
{
    private WaterGun waterGun;
    private Animator animator;

    public EnemyWatergunState(AIContext context)
    {
        waterGun = context.waterGun;
        animator = context.GetComponentInChildren<Animator>();
    }

    public void Enter()
    {
        waterGun.StartWaterGun();
        if (animator != null)
        {
            animator.SetTrigger("StartDefuse");
        }
    }

    public void Exit()
    {
        waterGun.StopWaterGun();
    }

    public void Update()
    {
        waterGun.Water();
    }
}
