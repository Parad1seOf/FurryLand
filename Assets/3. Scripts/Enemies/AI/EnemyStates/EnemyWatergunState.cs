using UnityEngine;

public class EnemyWatergunState : IEnemyState
{
    private WaterGun waterGun;

    public EnemyWatergunState(AIContext context)
    {
        waterGun = context.waterGun;
    }

    public void Enter()
    {
        waterGun.StartWaterGun();
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
