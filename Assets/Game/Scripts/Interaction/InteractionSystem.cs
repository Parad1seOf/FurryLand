// Gestiona la detección y el hold timer de interacción.
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class InteractionSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("Settings")]
    public float     interactRange = 2f;
    public LayerMask interactMask  = ~0;

    [Header("References")]
    public HUDManager hudManager;

    #endregion

    #region Private State

    private InputReader      input;
    private PlayerController player;
    private IInteractable    currentTarget;
    private Interactable     currentInteractable;  // versión concreta para leer holdToInteract
    private float            holdTimer;

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
        HandleInteraction();
    }

    #endregion

    #region Detection

    private void DetectInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange,
                                                interactMask, QueryTriggerInteraction.Collide);

        IInteractable closest     = null;
        Interactable  closestComp = null;
        float         closestDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist  = dist;
                closest      = interactable;
                closestComp  = col.GetComponentInParent<Interactable>();
            }
        }

        // Si cambia el objetivo, resetea el timer
        if (closest != currentTarget)
        {
            holdTimer = 0f;
            hudManager?.SetHoldProgress(0f, false);
        }

        currentTarget      = closest;
        currentInteractable = closestComp;
        hudManager?.SetInteractLabel(currentTarget?.InteractLabel);
    }

    #endregion

    #region Interaction

    private void HandleInteraction()
    {
        if (currentTarget == null)
        {
            ResetHold();
            return;
        }

        bool needsHold = currentInteractable != null && currentInteractable.HoldToInteract;

        if (needsHold)
        {
            if (input.InteractHeld)
            {
                float duration = currentInteractable.HoldDuration;
                holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(holdTimer / duration);
                hudManager?.SetHoldProgress(progress, true);

                if (holdTimer >= duration)
                {
                    currentTarget.Interact(player);
                    ResetHold();
                }
            }
            else
            {
                // Soltó E antes de completar — resetea
                ResetHold();
            }
        }
        else
        {
            // Interacción instantánea
            if (input.InteractPressed)
                currentTarget.Interact(player);
        }
    }

    private void ResetHold()
    {
        holdTimer = 0f;
        hudManager?.SetHoldProgress(0f, false);
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