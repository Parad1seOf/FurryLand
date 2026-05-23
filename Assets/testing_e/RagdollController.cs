using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [Header("Refs principales (desactivar al hacer ragdoll)")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider mainCollider;

    [Header("Huesos del ragdoll")]
    [Tooltip("Si está vacío se auto-rellena con todos los Rigidbody hijos.")]
    [SerializeField] private Rigidbody[] ragdollBodies;
    [Tooltip("Si está vacío se auto-rellena con todos los Collider hijos (excluye mainCollider).")]
    [SerializeField] private Collider[] ragdollColliders;

    [Header("Comportamiento")]
    [Tooltip("Si true, al activar ragdoll resetea la velocidad de los huesos. Déjalo en false si quieres conservar la inercia del cuerpo (correr, caer, etc.).")]
    [SerializeField] private bool zeroVelocityOnEnable = false;

    private void Awake()
    {
        if (ragdollBodies == null || ragdollBodies.Length == 0)
            ragdollBodies = GetComponentsInChildren<Rigidbody>(true);

        if (ragdollColliders == null || ragdollColliders.Length == 0)
            ragdollColliders = GetComponentsInChildren<Collider>(true);

        SetRagdoll(false);
    }

    public void SetRagdoll(bool active)
    {
        if (animator     != null) animator.enabled     = !active;
        if (agent        != null) agent.enabled        = !active;
        if (mainCollider != null) mainCollider.enabled = !active;

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == null) continue;
            rb.isKinematic      = !active;
            rb.detectCollisions = active;
            if (active && zeroVelocityOnEnable) rb.linearVelocity = Vector3.zero;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col == null || col == mainCollider) continue;
            col.enabled = active ? true : col.enabled;
        }
    }

    public void AddExplosionForce(Vector3 origin, float force, float radius, float upMod)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == null || rb.isKinematic) continue;
            rb.AddExplosionForce(force, origin, radius, upMod);
        }
    }

    public Rigidbody GetClosestBone(Vector3 worldPos)
    {
        Rigidbody best = null;
        float bestDist = float.MaxValue;
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == null) continue;
            float d = (rb.worldCenterOfMass - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = rb; }
        }
        return best;
    }
}