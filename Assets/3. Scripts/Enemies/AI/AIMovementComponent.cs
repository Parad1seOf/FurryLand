using UnityEngine;
using UnityEngine.AI;

public class AIMovementComponent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float moveSpeed = 3;

    [SerializeField] private float rotationSpeed;

    public void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        agent.Warp(transform.position);
        agent.speed = moveSpeed;
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
}
