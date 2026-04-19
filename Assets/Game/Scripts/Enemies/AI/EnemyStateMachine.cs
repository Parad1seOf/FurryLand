using System.ComponentModel;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IChangeState
{
    private IEnemyState currentState;
    private IEnemyState previousState;

    [SerializeField] private DetectionComponent detection;



    [SerializeField] private string state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        ChangeState(new EnemyIdleState(new AIContext(this, detection)));
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
        state = currentState.GetType().Name;
    }

    public void ChangeState(IEnemyState newState)
    {
        previousState = currentState;
        previousState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public DetectionComponent GetDetectionComponent()
    {
        return detection;
    }
}
