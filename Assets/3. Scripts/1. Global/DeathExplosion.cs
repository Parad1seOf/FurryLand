using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class DeathExplosion : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float vfxLifetime = 5f;

    [Header("Blood VFX (VFX Graph)")]
    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private float bloodVFXYOffset = -1.8f;
    [SerializeField] private Vector3 bloodVFXEulerRotation;
    [SerializeField] private bool bloodVFXFlattenDirection = true;
    [SerializeField] private float bloodVFXLifetime = 5f;

    [Header("Cabeza")]
    [SerializeField] private GameObject attachedHead;
    [SerializeField] private GameObject[] attachedHeadExtras;
    [SerializeField] private Transform headBone;
    [SerializeField] private GameObject detachedHeadPrefab;
    [SerializeField] private float headLaunchForce = 6f;
    [SerializeField] private float headExtraTorque = 8f;
    [SerializeField] private float detachedHeadLifetime = 8f;

    [Header("Ragdoll")]
    [SerializeField] private RagdollController ragdoll;
    [SerializeField] private float hitImpulseMultiplier = 1f;

    [Header("Bodyshot")]
    [SerializeField] private float bodyDespawnDelay = 6f;

    [Header("Empuje en area")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float upwardsModifier = 1f;
    [SerializeField] private LayerMask affectedLayers = ~0;

    [Header("Dano en area")]
    [SerializeField] private float damageRadius = 0f;
    [SerializeField] private float damageAmount = 0f;

    private HealthSystem health;
    private BodyPartType lastHitPart = BodyPartType.Default;
    private Vector3      lastHitPoint;
    private Vector3      lastHitDirection;
    private float        lastHitForce;
    private Coroutine    despawnRoutine;

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

    public void NotifyHit(BodyPartType part, Vector3 hitPoint, Vector3 hitDirection, float force)
    {
        bool keepAsHead = (lastHitPart == BodyPartType.Head && part != BodyPartType.Head);

        lastHitPart      = keepAsHead ? BodyPartType.Head : part;
        lastHitPoint     = hitPoint;
        lastHitDirection = hitDirection.sqrMagnitude > 0.0001f ? hitDirection.normalized : -transform.forward;
        lastHitForce     = force;
    }

    private void OnDeath()
    {
        SpawnBloodVFX();

        if (lastHitPart == BodyPartType.Head) DeathByHeadshot();
        else                                  DeathByBodyshot();
    }

    private void DeathByHeadshot()
    {
        Vector3    spawnPos = transform.position + Vector3.up * 1.7f;
        Quaternion spawnRot = transform.rotation;
        if (headBone != null) { spawnPos = headBone.position; spawnRot = headBone.rotation; }

        SetHeadPiecesActive(false);

        if (detachedHeadPrefab != null)
        {
            GameObject head = Instantiate(detachedHeadPrefab, spawnPos, spawnRot);
            Rigidbody  rb   = head.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 baseDir = lastHitDirection.sqrMagnitude > 0.0001f
                                  ? lastHitDirection
                                  : -transform.forward;
                Vector3 dir = (baseDir + Vector3.up * 1.2f).normalized;
                rb.AddForce (dir * headLaunchForce,                     ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * headExtraTorque, ForceMode.Impulse);
            }
            if (detachedHeadLifetime > 0f) Destroy(head, detachedHeadLifetime);
        }

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, spawnPos, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }

        if (ragdoll != null) ragdoll.SetRagdoll(true);
        ApplyHitImpulseToRagdoll();

        ApplyAreaEffects();
    }

    private void DeathByBodyshot()
    {
        if (ragdoll != null) ragdoll.SetRagdoll(true);
        ApplyHitImpulseToRagdoll();

        ApplyAreaEffects();

        if (despawnRoutine != null) StopCoroutine(despawnRoutine);
        despawnRoutine = StartCoroutine(DespawnRoutine(bodyDespawnDelay));
    }

    private void SpawnBloodVFX()
    {
        if (bloodVFX == null) return;

        Vector3 pos = transform.position + new Vector3(0f, bloodVFXYOffset, 0f);

        Vector3 dir = lastHitDirection.sqrMagnitude > 0.0001f
                      ? lastHitDirection
                      : -transform.forward;

        if (bloodVFXFlattenDirection)
        {
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = -transform.forward;
        }

        Quaternion rot = Quaternion.LookRotation(dir.normalized)
                         * Quaternion.Euler(bloodVFXEulerRotation);

        GameObject vfx = Instantiate(bloodVFX, pos, rot);
        if (bloodVFXLifetime > 0f) Destroy(vfx, bloodVFXLifetime);
    }

    private void ApplyHitImpulseToRagdoll()
    {
        if (ragdoll == null || lastHitForce <= 0f) return;

        Rigidbody target = ragdoll.GetClosestBone(lastHitPoint);
        if (target == null) return;

        target.AddForceAtPosition(lastHitDirection * lastHitForce * hitImpulseMultiplier,
                                  lastHitPoint,
                                  ForceMode.Impulse);
    }

    private void SetHeadPiecesActive(bool active)
    {
        if (attachedHead != null) attachedHead.SetActive(active);

        if (attachedHeadExtras != null)
        {
            for (int i = 0; i < attachedHeadExtras.Length; i++)
            {
                if (attachedHeadExtras[i] != null)
                    attachedHeadExtras[i].SetActive(active);
            }
        }
    }

    private IEnumerator DespawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        despawnRoutine = null;
    }

    private void ApplyAreaEffects()
    {
        if (explosionRadius > 0f && explosionForce > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);
            foreach (Collider hit in hits)
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb == null) continue;
                if (rb.transform == transform || rb.transform.IsChildOf(transform)) continue;
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }
        }

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

    public void Reset()
    {
        if (despawnRoutine != null) { StopCoroutine(despawnRoutine); despawnRoutine = null; }

        lastHitPart      = BodyPartType.Default;
        lastHitPoint     = Vector3.zero;
        lastHitDirection = Vector3.zero;
        lastHitForce     = 0f;

        SetHeadPiecesActive(true);
        if (ragdoll != null) ragdoll.SetRagdoll(false);
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
}