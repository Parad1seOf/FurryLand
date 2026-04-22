using UnityEngine;
using UnityEngine.AI;

public class EnemyAlertState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private float timer = 1f;
    private float time = 0f;

    public EnemyAlertState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
    }

    public void Enter()
    {
        AlertSystem alert = AlertSystem.Instance;
        if (!alert.IsAlreadyTriggered) alert.TriggerAlert();

        time = timer;
        detection.transform.forward = detection.GetTargetDirection();
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