using UnityEngine;
using UnityEngine.AI;

public class AIContext
{
    public IChangeState        changeState;
    public DetectionComponent  detection;
    public SuspicionComponent  suspicion;
    public Transform           playerTransform;
    public NavMeshAgent        agent;
    public Transform[]         waypoints;

    public AIContext(IChangeState       changeState,
                     DetectionComponent detection,
                     SuspicionComponent suspicion,
                     Transform          playerTransform,
                     NavMeshAgent       agent,
                     Transform[]        waypoints)
    {
        this.changeState     = changeState;
        this.detection       = detection;
        this.suspicion       = suspicion;
        this.playerTransform = playerTransform;
        this.agent           = agent;
        this.waypoints       = waypoints;
    }
}