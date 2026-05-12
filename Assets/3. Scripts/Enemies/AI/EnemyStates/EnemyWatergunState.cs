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
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        waterGun.Water();
    }
}
