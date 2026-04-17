// Detecta objetos interactuables en un radio alrededor del jugador (sin raycast).
// No hace falta mirar al objeto para interactuar.
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class InteractionSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("Settings")]
    public float     interactRange = 2f;
    public LayerMask interactMask  = ~0;

    [Header("References")]
    public HUDManager hudManager;   // opcional, para mostrar el label

    #endregion

    #region Private State

    private InputReader      input;
    private PlayerController player;
    private IInteractable    currentTarget;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        input  = GetComponent<InputReader>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        DetectInteractable();

        if (currentTarget != null && input.InteractPressed)
            currentTarget.Interact(player);
    }

    #endregion

    #region Private Methods

    private void DetectInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange,
                                                interactMask,
                                                QueryTriggerInteraction.Collide);

        IInteractable closest    = null;
        float         closestDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest     = interactable;
            }
        }

        currentTarget = closest;
        hudManager?.SetInteractLabel(currentTarget?.InteractLabel);
    }

    #endregion

    #region Public API

    public IInteractable CurrentTarget => currentTarget;

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = currentTarget != null ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
}