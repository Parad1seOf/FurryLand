using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;
    private IAIBehaviour behaviour;

    public EnemyIdleState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        changeState = context.changeState;
        behaviour = context.behaviour;
    }

    public void Enter()
    {
        if (context.idle != null)
            context.idle.active = true;
    }

    public void Exit()
    {
        if (context.idle != null)
            context.idle.active = false;
    }

    public void Update()
    {
        LookForSuspiciousActivity();


        //Algo mas?
        if (changeState == null)
            Debug.Log("noup");
    }

    private void LookForSuspiciousActivity()
    {
        if (detection.SeesSuspiciousConduct())
            changeState.ChangeState(behaviour.OnSuspicion());

        if (detection.PlayerIsTooClose())
            changeState.ChangeState(behaviour.OnAlert());
    }
}
