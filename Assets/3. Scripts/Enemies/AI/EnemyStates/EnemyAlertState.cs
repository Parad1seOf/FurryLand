using UnityEngine;
using UnityEngine.AI;

public class EnemyAlertState : IEnemyState
{
    private AIContext context;
    private float timer = 1f;
    private float time = 0f;

    public EnemyAlertState(AIContext context)
    {
        this.context = context;
    }

    public void Enter()
    {
        time = timer;
        //Mostrar exclamacion
    }

    public void Exit()
    {
        //Ocultar exclamacion
    }

    public void Update()
    {
        time -= Time.deltaTime;

        if (time < 0f) {
            context.changeState.ChangeState(new EnemyTravelState(context));
        }
    }
}