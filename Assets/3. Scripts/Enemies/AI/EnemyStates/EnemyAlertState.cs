using UnityEngine;
using UnityEngine.AI;

public class EnemyAlertState : IEnemyState
{
    private AIContext context;
    private DetectionComponent detection;
    private EnemyDisplay display;
    private float time = 1f;
    private float timer = 0f;
    private IAIBehaviour behaviour;

    public EnemyAlertState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        display = context.display;
        behaviour = context.behaviour;
    }

    public void Enter()
    {
        AlertSystem alert = AlertSystem.Instance;
        if (!alert.IsAlreadyTriggered) alert.TriggerAlert();

        timer = time;
        context.movement.LookAt(detection.GetTargetPosition());
        display.ChangeLabel("!", Color.red);
    }

    public void Exit()
    {
        display.ChangeLabel("", Color.white);
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0f) {
            context.changeState.ChangeState(behaviour.AfterAlert());
        }
    }
}