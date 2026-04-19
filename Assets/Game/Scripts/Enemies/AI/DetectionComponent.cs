using System;
using Unity.VisualScripting;
using UnityEngine;

public class DetectionComponent : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float escapeDetectionDistance;
    [SerializeField] private float suspiciousDistance;

    [SerializeField] private float actionDistance;

    [SerializeField] private SuspicionComponent playerSus;
    [SerializeField] private ITarget playerTarget;
    private Transform playerPos;

    public void Start()
    {
        //ESTO ES JODIDAMENTE HORRIBLE Y HAY QUE CAMBIARLO
        if (playerSus == null)
            playerSus = GameObject.FindGameObjectWithTag("Player").GetComponent<SuspicionComponent>();
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<ITarget>();



        playerPos = playerTarget.GetTransform();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeDetectionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, suspiciousDistance);

        Gizmos.color = Color.pink;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*detectionDistance);
    }

    public bool SeesSuspiciousConduct()
    {
        return playerSus.isSuspicious() && SeesPlayer() && IsPlayerInDetectionDistance();
    }

    public bool HasPlayerEscapedSuspicion()
    {
        return !playerSus.isSuspicious() || !SeesPlayer() || PlayerEscapedDetection();
    }

    public bool SeesPlayer()
    {
        Vector3 playerDirection = playerPos.position - transform.position;
        float angle = Vector3.Angle(transform.forward, playerDirection);
        return angle <= viewAngle * 0.5f;
    }

    public bool IsPlayerInDetectionDistance()
    {
        return (playerPos.position - transform.position).magnitude < detectionDistance;
    }

    public bool PlayerEscapedDetection()
    {
        return (playerPos.position - transform.position).magnitude > escapeDetectionDistance;
    }

    public bool PlayerIsTooClose()
    {
        return (playerPos.position - transform.position).magnitude < suspiciousDistance;
    }

    public bool PlayerIsInActionDistance()
    {
        return (playerPos.position - transform.position).magnitude < actionDistance;
    }
}
