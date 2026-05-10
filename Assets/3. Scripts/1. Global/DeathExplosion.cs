using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class DeathExplosion : MonoBehaviour
{
    [Header("VFX")]
    [Tooltip("Prefab con ParticleSystem (Collision ON) + BloodDecalSpawnerV2")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float vfxLifetime = 5f;

    [Header("Empuje (opcional)")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float upwardsModifier = 1f;
    [SerializeField] private LayerMask affectedLayers = ~0;

    [Header("Daño en área (opcional)")]
    [Tooltip("0 = sin daño")]
    [SerializeField] private float damageRadius = 0f;
    [SerializeField] private float damageAmount = 0f;

    private HealthSystem health;

    private void Start()
    {
        health = GetComponent<HealthSystem>();
        health.OnDeath += Explode;
    }

    private void OnDestroy()
    {
        if (health != null) health.OnDeath -= Explode;
    }

    private void Explode()
    {
        // 1. VFX (particulas de sangre + decals al impactar)
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }

        // 2. Empuje a rigidbodies cercanos
        if (explosionRadius > 0f && explosionForce > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);
            foreach (Collider hit in hits)
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }
        }

        // 3. Daño en área (opcional)
        if (damageRadius > 0f && damageAmount > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius, affectedLayers);
            foreach (Collider hit in hits)
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;
                IDamageable dmg = hit.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(damageAmount);
            }
        }
    }
}