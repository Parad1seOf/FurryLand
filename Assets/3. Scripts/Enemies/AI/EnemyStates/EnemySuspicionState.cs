using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemySuspicionState : IEnemyState
{
    private AIContext context;
    private IChangeState changeState;
    private DetectionComponent detection;
    private EnemyDisplay display;


    private float time = 100f;
    private float timer;
    private float duration;

    public EnemySuspicionState(AIContext context)
    {
        this.context = context;
        detection = context.detection;
        changeState = context.changeState;
        display = context.display;
    }

    public void Enter()
    {
        timer = 0;
    }

    public void Exit()
    {
        display.ChangeLabel("", Color.white);
    }

    public  void Update()
    {
        UpdateSuspicionProgress();
        CheckPlayer();

        Rotate();
        UpdateDisplay();
    }

    private void UpdateSuspicionProgress()
    {
        timer += detection.GetPlayerSuspicionLevel() * Time.deltaTime;
        if (timer >= time)
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
        string str = "";
        if (timer > 75)
        {
            str = "???";
        }
        else if (timer > 50)
        {
            str = "??";
        }
        else if (timer > 25)
        {
            str = "??";
        }

        display.ChangeLabel(str, Color.darkViolet);
    }
}
