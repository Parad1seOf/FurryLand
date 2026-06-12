using UnityEngine;
using UnityEngine.AI;

using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [Header("Refs principales (desactivar al hacer ragdoll)")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider[] mainColliders;

    [Header("Huesos del ragdoll")]
    [Tooltip("Si está vacío se auto-rellena con todos los Rigidbody hijos.")]
    [SerializeField] private Rigidbody[] ragdollBodies;

    [Tooltip("Si está vacío se auto-rellena con todos los Collider hijos.")]
    [SerializeField] private Collider[] ragdollColliders;

    [Header("Comportamiento")]
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
        if (animator != null)
            animator.enabled = !active;

        if (agent != null)
            agent.enabled = !active;

        if (mainColliders != null)
        {
            foreach (Collider col in mainColliders)
            {
                if (col == null) continue;
                col.enabled = !active;
            }
        }

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == null) continue;

            rb.isKinematic = !active;
            rb.detectCollisions = active;

            if (active && zeroVelocityOnEnable)
                rb.linearVelocity = Vector3.zero;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col == null) continue;
            if (IsMainCollider(col)) continue;

            col.enabled = active;
        }
    }

    private bool IsMainCollider(Collider col)
    {
        if (mainColliders == null) return false;

        foreach (Collider main in mainColliders)
        {
            if (main == col)
                return true;
        }

        return false;
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

            if (d < bestDist)
            {
                bestDist = d;
                best = rb;
            }
        }

        return best;
    }
}