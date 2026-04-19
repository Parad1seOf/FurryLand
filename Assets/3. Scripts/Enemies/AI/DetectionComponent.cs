using UnityEngine;

public class DetectionComponent : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float viewAngle               = 45f;
    [SerializeField] private float detectionDistance       = 5f;
    [SerializeField] private float escapeDetectionDistance = 7f;
    [SerializeField] private float actionDistance          = 2.5f;

    [Header("Visión")]
    [SerializeField] private Vector3   eyeOffset  = new Vector3(0f, 0.8f, 0f);
    [SerializeField] private LayerMask visionMask = ~0;

    [Header("Sonido")]
    [SerializeField] private float hearingDistance = 10f;

    [Header("Bajar sospecha")]
    [SerializeField] private float lowerRate = 20f;

    private Transform                  playerPos;
    private SuspiciousActionsComponent playerActions;
    private SuspicionComponent         suspicion;

    // Llamado desde EnemyStateMachine.Awake()
    public void Init(Transform player, SuspiciousActionsComponent actions)
    {
        playerPos     = player;
        playerActions = actions;
        suspicion     = GetComponent<SuspicionComponent>();

        if (playerActions != null)
        {
            playerActions.OnSoundAction  += OnSoundHeard;
            playerActions.OnVisionAction += OnVisionDetected;
        }
    }

    private void OnDestroy()
    {
        if (playerActions != null)
        {
            playerActions.OnSoundAction  -= OnSoundHeard;
            playerActions.OnVisionAction -= OnVisionDetected;
        }
    }

    private void OnSoundHeard(float amount)
    {
        if (suspicion == null) return;
        if (IsPlayerInHearingDistance())
            suspicion.RiseSuspicion(amount);
    }

    private void OnVisionDetected(float amount)
    {
        if (suspicion == null) return;
        if (SeesPlayer() && IsPlayerInDetectionDistance())
            suspicion.RiseSuspicion(amount);
    }

    public void TickSuspicion(SuspicionComponent sus)
    {
        if (playerPos == null) return;

        if (!SeesPlayer() || !IsPlayerInDetectionDistance())
        {
            sus.LowerSuspicion(lowerRate * Time.deltaTime);
            return;
        }

        float rate = playerActions != null ? playerActions.GetContinuousRate() : 0f;

        if (rate > 0f)
            sus.RiseSuspicion(rate * Time.deltaTime);
        else
            sus.LowerSuspicion(lowerRate * Time.deltaTime);
    }

    public bool SeesPlayer()
    {
        if (playerPos == null) return false;

        Vector3 origin = transform.position + eyeOffset;
        Vector3 dir    = playerPos.position - origin;
        float   angle  = Vector3.Angle(transform.forward, dir);

        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit,
                            detectionDistance, visionMask))
        {
            return hit.transform == playerPos ||
                   hit.transform.IsChildOf(playerPos);
        }

        return false;
    }

    public bool IsPlayerInDetectionDistance() =>
        playerPos != null &&
        (playerPos.position - transform.position).magnitude < detectionDistance;

    public bool IsPlayerInHearingDistance() =>
        playerPos != null &&
        (playerPos.position - transform.position).magnitude < hearingDistance;

    public bool PlayerEscapedDetection() =>
        playerPos == null ||
        (playerPos.position - transform.position).magnitude > escapeDetectionDistance;

    public bool PlayerIsInActionDistance() =>
        playerPos != null &&
        (playerPos.position - transform.position).magnitude < actionDistance;

    private void OnDrawGizmosSelected()
    {
        Vector3 origin    = transform.position + eyeOffset;
        float   halfAngle = viewAngle * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeDetectionDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, actionDistance);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, hearingDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + transform.forward * detectionDistance);

        Vector3 leftDir  = Quaternion.Euler(0f, -halfAngle, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f,  halfAngle, 0f) * transform.forward;
        Gizmos.DrawLine(origin, origin + leftDir  * detectionDistance);
        Gizmos.DrawLine(origin, origin + rightDir * detectionDistance);

        int steps = 10;
        for (int i = 0; i <= steps; i++)
        {
            float   t   = (float)i / steps;
            float   a   = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0f, a, 0f) * transform.forward;
            Vector3 end = origin + dir * detectionDistance;

            if (i > 0)
            {
                float   tPrev   = (float)(i - 1) / steps;
                float   aPrev   = Mathf.Lerp(-halfAngle, halfAngle, tPrev);
                Vector3 dirPrev = Quaternion.Euler(0f, aPrev, 0f) * transform.forward;
                Vector3 endPrev = origin + dirPrev * detectionDistance;
                Gizmos.DrawLine(end, endPrev);
            }

            Gizmos.DrawLine(origin, end);
        }
    }
}