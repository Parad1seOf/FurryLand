using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class DetectionComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Angulo de vision del enemigo.")]
    private float viewAngle = 45f;

    [SerializeField]
    [Tooltip("A que distancia el enemigo detecta a su objetivo.")]
    private float detectionDistance = 5f;

    [SerializeField]
    [Tooltip("A que distancia tiene que estar el jugador como minimo para escapar de las sospechas una vez ha parecido sospechoso.")]
    private float escapeDetectionDistance = 7f;

    [SerializeField]
    [Tooltip("A que distancia el enemigo empieza a sospechar del jugador sin ningun motivo.")]
    private float suspiciousDistance = 2f;


    [SerializeField]
    [Tooltip("A que distancia el enemigo puede atacar al jugador.")]
    private float attackDistance = 2.5f;

    [SerializeField]
    private float escapeAttackDistance = 3f;

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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.pink;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*detectionDistance);
    }

    public bool SeesSuspiciousConduct()
    {
        return playerSus.IsSuspicious() && SeesPlayer() && IsPlayerInDetectionDistance();
    }

    public bool HasPlayerEscapedSuspicion()
    {
        return !playerSus.IsSuspicious() || !SeesPlayer() || PlayerEscapedDetection();
    }

    public bool SeesPlayer()
    {
        Vector3 playerDirection = playerPos.position - transform.position;
        float angle = Vector3.Angle(transform.forward, playerDirection);
        if (!HasLineOfSight()) return false;
        return angle <= viewAngle * 0.5f;
    }

    public bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {

            ITarget target = hit.collider.GetComponent<ITarget>();

            return ReferenceEquals(this.playerTarget, target);
        }
        return false;
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
        return (playerPos.position - transform.position).magnitude < attackDistance;
    }

    public Vector3 GetTargetPosition()
    {
        return playerPos.position;
    }

    public Vector3 GetTargetDirection()
    {
        return playerPos.position - transform.position;
    }

    public float GetPlayerSuspicionLevel()
    {
        if (playerSus == null) return 0;

        return playerSus.GetSuspicionLevel();
    }

    public bool PlayerEscapedAttack()
    {
        return (playerPos.position - transform.position).magnitude > escapeAttackDistance;
    }
}
