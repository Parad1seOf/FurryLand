using UnityEngine;
using UnityEngine.AI;

public class AIMovementComponent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float moveSpeed = 3;

    [SerializeField] private float rotationSpeed = 3;

    [SerializeField] private Animator animator;
    //Para el turn
    [SerializeField] private float turnAnimationMinAnglePerFrame = 0.4f;
    [SerializeField] private float turnAnimationMaxMoveSpeed = 0.1f;

    private float lastYRotation;

    //IdleTypes and breaks
    [SerializeField] private float idleType = 0f;
    private float defaultIdleType;

    [Header("Idle Breaks")]
    [SerializeField] private bool useIdleBreaks = false;
    [SerializeField] private float minIdleBreakTime = 4f;
    [SerializeField] private float maxIdleBreakTime = 9f;

    private float idleBreakTimer;
    private bool usingCombatIdle;

    public void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Start()
    {
        agent.Warp(transform.position);
        agent.speed = moveSpeed;

        lastYRotation = transform.eulerAngles.y; // para el turn

        defaultIdleType = idleType;

        if (animator != null)
            animator.SetFloat("IdleType", idleType);

        ResetIdleBreakTimer();
    }

    private void Update()
    {
        //if (animator == null || agent == null) return;

        //float speed01 = agent.velocity.magnitude / moveSpeed;
        //speed01 = Mathf.Clamp01(speed01);

        //animator.SetFloat("Speed", speed01);

        // Con el turn 

        if (animator == null || agent == null) return;

        float speed01 = agent.velocity.magnitude / moveSpeed;
        speed01 = Mathf.Clamp01(speed01);

        animator.SetFloat("Speed", speed01);

        float currentYRotation = transform.eulerAngles.y;
        float turnAmount = Mathf.Abs(Mathf.DeltaAngle(lastYRotation, currentYRotation));

        bool isAlmostStopped = agent.velocity.magnitude <= turnAnimationMaxMoveSpeed;
        bool isRotatingEnough = turnAmount >= turnAnimationMinAnglePerFrame;

        bool isTurning = isAlmostStopped && isRotatingEnough;

        animator.SetBool("IsTurning", isTurning);

        lastYRotation = currentYRotation;

        UpdateIdleBreaks(speed01);

    }

    public void MoveTo(Vector3 destination)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void LookAtDirection(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    //Idle Breaks

    private void UpdateIdleBreaks(float speed01)
    {
        if (!useIdleBreaks) return;
        if (animator == null) return;
        if (usingCombatIdle) return;

        bool isIdle = speed01 < 0.05f;
        bool isTurning = animator.GetBool("IsTurning");

        if (!isIdle || isTurning)
        {
            ResetIdleBreakTimer();
            return;
        }

        idleBreakTimer -= Time.deltaTime;

        if (idleBreakTimer > 0f)
            return;

        TryPlayIdleBreak();
        ResetIdleBreakTimer();
    }

    private void TryPlayIdleBreak()
    {
        int type = Mathf.RoundToInt(idleType);

        Debug.Log("Intentando IdleBreak. IdleType = " + type);

        if (type == 4)
        {
            animator.SetTrigger("EatBreak");
        }
        else if (type == 0 || type == 3)
        {
            animator.SetTrigger("WaveBreak");
        }
    }

    private void ResetIdleBreakTimer()
    {
        idleBreakTimer = Random.Range(minIdleBreakTime, maxIdleBreakTime);
    }

    public void UseDefaultIdle()
    {
        usingCombatIdle = false;

        idleType = defaultIdleType;

        if (animator != null)
            animator.SetFloat("IdleType", idleType);

        ResetIdleBreakTimer();
    }

    public void UseCombatIdle()
    {
        usingCombatIdle = true;

        idleType = 0f;

        if (animator != null)
            animator.SetFloat("IdleType", idleType);

        ResetIdleBreakTimer();
    }
}
