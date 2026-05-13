using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [Header("Refs principales (desactivar al hacer ragdoll)")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider mainCollider;   // el collider "principal" del enemigo, el de detección/movimiento

    [Header("Huesos del ragdoll")]
    [Tooltip("Si está vacío se auto-rellena con todos los Rigidbody hijos.")]
    [SerializeField] private Rigidbody[] ragdollBodies;
    [Tooltip("Si está vacío se auto-rellena con todos los Collider hijos (excluye mainCollider).")]
    [SerializeField] private Collider[] ragdollColliders;

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
            rb.isKinematic     = !active;
            rb.detectCollisions = active;
            if (active) rb.linearVelocity = Vector3.zero;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col == null || col == mainCollider) continue;
            // los colliders del ragdoll están desactivados mientras anima (no chocan entre sí),
            // pero se activan al morir para que el ragdoll choque con el mundo.
            // Si quieres mantenerlos siempre activos para que el raycast los detecte vivo,
            // comenta la siguiente línea.
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
}