using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour, IChangeState
{
    private IEnemyState currentState;
    private IEnemyState previousState;
    private AIContext   context;

    [SerializeField] private DetectionComponent detection;
    [SerializeField] private SuspicionComponent  suspicion;
    [SerializeField] private NavMeshAgent        agent;
    [SerializeField] private Transform[]         waypoints;

    [SerializeField] private string state; // debug

    public string CurrentStateName => currentState?.GetType().Name ?? "None";
    public float  SuspicionLevel   => suspicion != null ? suspicion.GetSuspicionLevel() : 0f;

    private void Awake()
    {
        if (detection == null) detection = GetComponent<DetectionComponent>();
        if (suspicion  == null) suspicion  = GetComponent<SuspicionComponent>();
        if (agent      == null) agent      = GetComponent<NavMeshAgent>();

        // Inyectar player en DetectionComponent — centralizado aquí
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        Transform                  playerTransform = playerGO?.transform;
        SuspiciousActionsComponent playerActions   = playerGO?.GetComponent<SuspiciousActionsComponent>();

        if (detection != null && playerTransform != null)
            detection.Init(playerTransform, playerActions);

        // Contexto único — se reutiliza en todos los estados
        context = new AIContext(this, detection, suspicion,
                                playerTransform, agent, waypoints);

        // Si tiene waypoints empieza patrullando, si no en idle
        ChangeState(waypoints != null && waypoints.Length > 0
            ? (IEnemyState)new EnemyPatrolState(context)
            : new EnemyIdleState(context));
    }

    private void Start()
    {
        if (AlertSystem.Instance != null)
        {
            AlertSystem.Instance.OnAlertTriggered += GoToAlert;
            AlertSystem.Instance.Register(suspicion);
        }

        if (suspicion != null)
            suspicion.OnMaxSuspicion += OnMaxSuspicionReached;
    }

    private void OnDestroy()
    {
        if (AlertSystem.Instance != null)
        {
            AlertSystem.Instance.OnAlertTriggered -= GoToAlert;
            AlertSystem.Instance.Unregister(suspicion);
        }

        if (suspicion != null)
            suspicion.OnMaxSuspicion -= OnMaxSuspicionReached;
    }

    private void Update() => currentState?.Update();

    public void ChangeState(IEnemyState newState)
    {
        previousState = currentState;
        previousState?.Exit();
        currentState = newState;
        currentState.Enter();
        state = currentState.GetType().Name;
    }

    private void OnMaxSuspicionReached() => AlertSystem.Instance?.TriggerAlert();

    private void GoToAlert()
    {
        if (currentState is EnemyAlertState) return;
        ChangeState(new EnemyAlertState(context));
    }
}