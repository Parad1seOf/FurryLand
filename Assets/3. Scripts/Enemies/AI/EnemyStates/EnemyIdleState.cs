using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;

    public EnemyIdleState(AIContext context)
    {
        this.context = context;
        this.detection = context.detection;
        this.changeState = context.changeState;
    }

    public void Enter() {}

    public void Exit() {}

    public void Update()
    {
        LookForSuspiciousActivity();
        

        //Algo mas?
    }

    private void LookForSuspiciousActivity()
    {
        if (detection.PlayerIsTooClose())
            changeState.ChangeState(new EnemyAlertState(context));

        if (detection.SeesSuspiciousConduct())
            changeState.ChangeState(new EnemySuspicionState(context));
    }
}
