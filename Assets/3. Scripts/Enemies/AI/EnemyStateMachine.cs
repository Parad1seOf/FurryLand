using System.ComponentModel;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IChangeState
{
    private IEnemyState currentState;
    private IEnemyState previousState;

    [SerializeField] private DetectionComponent detection;
    [SerializeField] private AIMovementComponent movement;
    [SerializeField] private IAttack attack;
    private AIContext context;

    [SerializeField]
    [Tooltip("No tocar.")]
    private string state;

    void Start()
    {
        if (detection == null)
            detection = GetComponent<DetectionComponent>();
        if (movement == null)
            movement = GetComponent<AIMovementComponent>();
        if (attack == null)
            attack = GetComponent<IAttack>();

        context = new AIContext(this, detection, movement, attack);
        ChangeState(new EnemyIdleState(context));

        GetComponent<HealthSystem>().OnDeath += Die;
    }

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

    public void Alert()
    {
        ChangeState(new EnemyAlertState(context));
    }

    public void Die()
    {
        ChangeState(new EnemyDeadState(context));
        EnemyPool.instance.AddEnemy(gameObject);
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        ChangeState(new EnemyTravelState(context));
        GetComponent<HealthSystem>().Restore(1000);
    }
}
