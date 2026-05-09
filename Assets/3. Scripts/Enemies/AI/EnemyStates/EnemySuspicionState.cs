using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySuspicionState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;
    private EnemyDisplay display;
    private EnemyAwarenessComponent awareness;

    public EnemySuspicionState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        changeState = context.changeState;
        display = context.display;
        awareness = context.awareness;
    }

    public void Enter()
    {

    }

    public void Exit()
    {
        awareness.BecomeUnaware();

        display.ChangeLabel("", Color.white);
    }

    public  void Update()
    {
        UpdateDisplay();
        UpdateSuspicionProgress();
        CheckPlayer();

        Rotate();
        
    }

    private void UpdateSuspicionProgress()
    {
        if (awareness.UpdateAwareness(detection.GetPlayerSuspicionLevel()))
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

    private void UpdateDisplay()
    {
        display.ChangeLabel(Mathf.FloorToInt(awareness.GetAwareness()).ToSafeString(), Color.darkViolet);
    }
}
