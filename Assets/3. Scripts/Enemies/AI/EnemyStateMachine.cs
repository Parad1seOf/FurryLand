using System.ComponentModel;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IChangeState
{
    private IEnemyState currentState;
    private IEnemyState previousState;

    [SerializeField] private DetectionComponent detection;
    [SerializeField] private AIMovementComponent movement;

    [SerializeField]
    [Tooltip("No tocar.")]
    private string state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (detection == null)
            detection = GetComponent<DetectionComponent>();

        ChangeState(new EnemyIdleState(new AIContext(this, detection, movement)));
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        previousState = currentState;
        previousState?.Exit();
        currentState = newState;
        currentState.Enter();

        //Development
        state = currentState.GetType().Name;
    }
}
