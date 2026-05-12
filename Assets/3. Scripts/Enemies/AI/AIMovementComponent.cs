using UnityEngine;
using UnityEngine.AI;

public class AIMovementComponent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float moveSpeed = 3;

    public void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    public void MoveTo(Vector3 destination)
    {
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
