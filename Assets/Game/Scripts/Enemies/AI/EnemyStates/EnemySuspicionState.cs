using Unity.VisualScripting;
using UnityEngine;

public class EnemySuspicionState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;


    private float time = 60f;
    private float timer;

    public EnemySuspicionState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        changeState = context.changeState;
    }

    public void Enter()
    {
        timer = time;
    }

    public void Exit()
    {
        
    }

    public  void Update()
    {
        timer -= Time.deltaTime;
        if (detection.HasPlayerEscapedSuspicion())
            changeState.ChangeState(new EnemyIdleState(context));

        if (timer <= 0)
            //changeState.ChangeState(new EnemyIdleState(context))
            ;
    }
}
