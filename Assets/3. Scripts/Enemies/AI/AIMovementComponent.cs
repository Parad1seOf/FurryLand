using UnityEngine;
using UnityEngine.AI;

public class AIMovementComponent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float moveSpeed = 3;

    [SerializeField] private float rotationSpeed = 3;

    [SerializeField] private Animator animator;

    public void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Start()
    {
        agent.Warp(transform.position);
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (animator == null || agent == null) return;

        float speed01 = agent.velocity.magnitude / moveSpeed;
        speed01 = Mathf.Clamp01(speed01);

        animator.SetFloat("Speed", speed01);
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
}
