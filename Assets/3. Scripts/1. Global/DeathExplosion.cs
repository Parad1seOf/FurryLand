using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class DeathExplosion : MonoBehaviour
{
    [Header("VFX")]
    [Tooltip("Prefab con ParticleSystem (Collision ON) + BloodDecalSpawnerV2")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float vfxLifetime = 5f;

    [Header("Cabeza")]
    [Tooltip("GameObject de la cabeza skinneada (la separada en 3ds Max). Se oculta al recibir headshot.")]
    [SerializeField] private GameObject attachedHead;
    [Tooltip("Hueso de la cabeza — referencia para spawnear el VFX y la cabeza física en su posición.")]
    [SerializeField] private Transform headBone;
    [Tooltip("Prefab de la cabeza física: mesh + Rigidbody + Collider.")]
    [SerializeField] private GameObject detachedHeadPrefab;
    [SerializeField] private float headLaunchForce = 6f;
    [SerializeField] private float headExtraTorque = 8f;
    [SerializeField] private float detachedHeadLifetime = 8f;

    [Header("Ragdoll")]
    [SerializeField] private RagdollController ragdoll;

    [Header("Bodyshot — desaparición del cuerpo entero")]
    [SerializeField] private float bodyDespawnDelay = 6f;

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
    private BodyPartType lastHitPart = BodyPartType.Default;
    private Coroutine despawnRoutine;

    private void Awake()
    {
        if (ragdoll == null) ragdoll = GetComponent<RagdollController>();
    }

    private void Start()
    {
        health = GetComponent<HealthSystem>();
        health.OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        if (health != null) health.OnDeath -= OnDeath;
    }

    // Llamado por GunSystem.ProcessHit justo antes de aplicar daño.
    public void NotifyHit(BodyPartType part)
    {
        lastHitPart = part;
    }

    private void OnDeath()
    {
        if (lastHitPart == BodyPartType.Head) DeathByHeadshot();
        else                                  DeathByBodyshot();
    }

    private void DeathByHeadshot()
    {
        Vector3    spawnPos = transform.position + Vector3.up * 1.7f;
        Quaternion spawnRot = transform.rotation;
        if (headBone != null) { spawnPos = headBone.position; spawnRot = headBone.rotation; }

        // 1. Ocultar la cabeza pegada al cuerpo
        if (attachedHead != null) attachedHead.SetActive(false);

        // 2. Spawn cabeza física + impulso
        if (detachedHeadPrefab != null)
        {
            GameObject head = Instantiate(detachedHeadPrefab, spawnPos, spawnRot);
            Rigidbody  rb   = head.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (-transform.forward + Vector3.up * 1.2f).normalized;
                rb.AddForce (dir * headLaunchForce,                          ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * headExtraTorque,      ForceMode.Impulse);
            }
            if (detachedHeadLifetime > 0f) Destroy(head, detachedHeadLifetime);
        }

        // 3. VFX (partículas de sangre + decals)
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, spawnPos, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }

        // 4. Ragdoll del cuerpo (sin despawn)
        if (ragdoll != null) ragdoll.SetRagdoll(true);

        ApplyAreaEffects();
    }

    private void DeathByBodyshot()
    {
        if (ragdoll != null) ragdoll.SetRagdoll(true);

        ApplyAreaEffects();

        // Desaparecer el cuerpo entero tras X segundos
        if (despawnRoutine != null) StopCoroutine(despawnRoutine);
        despawnRoutine = StartCoroutine(DespawnRoutine(bodyDespawnDelay));
    }

    private IEnumerator DespawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        // OJO: no Destroy — EnemyStateMachine ya mete el gameObject al pool en Die().
        // Lo dejamos desactivado para que Spawner pueda reutilizarlo con Respawn().
        gameObject.SetActive(false);
        despawnRoutine = null;
    }

    private void ApplyAreaEffects()
    {
        // 1. Empuje a rigidbodies cercanos
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

        // 2. Daño en área (opcional)
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

    // Llamado desde EnemyStateMachine.Respawn() para reutilizar el enemigo del pool.
    public void Reset()
    {
        if (despawnRoutine != null) { StopCoroutine(despawnRoutine); despawnRoutine = null; }

        lastHitPart = BodyPartType.Default;
        if (attachedHead != null) attachedHead.SetActive(true);
        if (ragdoll != null) ragdoll.SetRagdoll(false);
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
}