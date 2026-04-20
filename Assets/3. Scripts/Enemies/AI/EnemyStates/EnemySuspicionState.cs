using Unity.VisualScripting;
using UnityEngine;

public class EnemySuspicionState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;


    private float time = 10f;
    private float timer;
    private float duration;

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
        UpdateSuspicionProgress();
        CheckPlayer();

        Rotate();
    }

    private void UpdateSuspicionProgress()
    {
        timer -= detection.GetPlayerSuspicionLevel() * Time.deltaTime;
        if (timer <= 0)
            changeState.ChangeState(new EnemyAlertState(context));
    }

    private void CheckPlayer()
    {
        if (detection.HasPlayerEscapedSuspicion())
            changeState.ChangeState(new EnemyIdleState(context));
    }

    private void Rotate()
    {
        //Mal
        detection.transform.forward = detection.GetTargetDirection();
    }
}
