using UnityEngine;

public class DetectionComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Angulo de vision del enemigo.")]
    private float viewAngle = 90f;

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
    [SerializeField] private Transform eyes;
    [SerializeField] private LayerMask ignoreLayers;

    [SerializeField] private GameObject detectingMark;

    public void Awake()
    {
        if (playerSus == null)
            playerSus = GameObject.FindGameObjectWithTag("Player").GetComponent<SuspicionComponent>();
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<ITarget>();
        if (eyes == null)
            eyes = transform;
        playerPos = playerTarget.GetTransform();
    }

    public void Update()
    {
        UpdateDetectionIcon();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyes.position, detectionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(eyes.position, escapeDetectionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(eyes.position, suspiciousDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(eyes.position, attackDistance);

        Gizmos.color = Color.pink;
        Gizmos.DrawLine(eyes.position, eyes.position + eyes.forward*detectionDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(eyes.position, escapeAttackDistance);
    }

    public bool SeesSuspiciousConduct()
    {
        return IsPlayerInDetectionDistance() && SeesPlayer() && playerSus.IsSuspicious();
    }

    public bool HasPlayerEscapedSuspicion()
    {
        return !playerSus.IsSuspicious() || !SeesPlayer() || PlayerEscapedDetection();
    }

    public bool SeesPlayer()
    {
        if (!HasLineOfSight())
        {
            return false;
        }

        Vector3 playerDirection = playerPos.position - eyes.position;
        playerDirection.y = 0;
        Vector3 direction = eyes.forward;
        direction.y = 0;
        float angle = Vector3.Angle(direction, playerDirection);
        return angle <= viewAngle * 0.5f;
    }

    public bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 direction = playerPos.position - eyes.position;

        if (Physics.Raycast(eyes.position, direction, out hit, detectionDistance, ~ignoreLayers))
        {
            ITarget target = hit.collider.GetComponent<ITarget>();
            return ReferenceEquals(playerTarget, target);
        }
        return false;
    }

    public bool IsPlayerInDetectionDistance()
    {
        return (playerPos.position - eyes.position).magnitude < detectionDistance;
    }

    public bool PlayerEscapedDetection()
    {
        return (playerPos.position - eyes.position).magnitude > escapeDetectionDistance;
    }

    public bool PlayerIsTooClose()
    {
        return (playerPos.position - eyes.position).magnitude < suspiciousDistance;
    }

    public bool PlayerIsInActionDistance()
    {
        return (playerPos.position - eyes.position).magnitude < attackDistance;
    }

    public Vector3 GetTargetPosition()
    {
        return playerPos.position;
    }

    public Vector3 GetTargetDirection()
    {
        return playerPos.position - eyes.position;
    }

    public float GetPlayerSuspicionLevel()
    {
        if (playerSus == null) return 0;

        return playerSus.GetSuspicionLevel();
    }

    public bool PlayerEscapedAttack()
    {
        return (playerPos.position - eyes.position).magnitude > escapeAttackDistance;
    }

    private void UpdateDetectionIcon()
    {
        bool seesPlayer = SeesPlayer();
        bool suspicious = playerSus != null && playerSus.IsSuspicious();

        if (detectingMark != null)
            detectingMark.SetActive(seesPlayer && !suspicious);
        
    }
}
