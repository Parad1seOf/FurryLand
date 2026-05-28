using System.ComponentModel;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IChangeState
{
    private IEnemyState currentState;
    private IEnemyState previousState;

    private IAIBehaviour behaviour;

    [SerializeField]
    [Tooltip("No tocar.")]
    private string state;

    void Awake()
    {
        behaviour = GetComponent<IAIBehaviour>();
        
    }

    void Start()
    {
        GetComponent<HealthSystem>().OnDeath += Die;
        if (currentState != null) return;
        if (AlertSystem.Instance.IsAlreadyTriggered) ChangeState(behaviour.OnRespawn());
        else ChangeState(behaviour.OnStart());

        if (!gameObject.activeSelf) ChangeState(behaviour.OnDie());

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

    public IEnemyState PreviousState() { return previousState; }

    public void Alert()
    {
        if (currentState is EnemyDeadState) return;
        ChangeState(behaviour.OnAlert());
    }

    public void Die()
    {
        if (ScoreManager.instance != null)
            ScoreManager.instance.scoreKill();

        ChangeState(behaviour.OnDie());
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        ChangeState(behaviour.OnRespawn());
        GetComponent<HealthSystem>().Restore(1000);
        GetComponent<DeathExplosion>()?.Reset();
    }
}