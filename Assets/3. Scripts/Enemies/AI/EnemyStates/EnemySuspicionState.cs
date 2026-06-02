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
    private IAIBehaviour behaviour;

    public EnemySuspicionState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        changeState = context.changeState;
        display = context.display;
        awareness = context.awareness;
        behaviour = context.behaviour;
    }

    public void Enter()
    {
        context.movement.Stop();
    }

    public void Exit()
    {
        awareness.BecomeUnaware();

        //display.ChangeLabel("", Color.white);
        display.HideSuspicion();
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
            changeState.ChangeState(behaviour.OnAlert());
    }

    private void CheckPlayer()
    {
        if (detection.HasPlayerEscapedSuspicion())
            changeState.ChangeState(changeState.PreviousState());
    }

    private void Rotate()
    {
        context.movement.LookAt(detection.GetTargetPosition());
    }

    private void UpdateDisplay()
    {
        //display.ChangeLabel(Mathf.FloorToInt(awareness.GetAwareness()).ToSafeString(), Color.darkViolet);

        display.ShowSuspicion(awareness.GetAwareness());
    }
}
