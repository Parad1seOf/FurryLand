using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class DeathExplosion : MonoBehaviour
{
    [Header("VFX")]
    [Tooltip("Prefab con ParticleSystem (Collision ON) + BloodDecalSpawnerV2")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float vfxLifetime = 5f;

    [Header("Blood VFX (VFX Graph)")]
    [Tooltip("Nuevo prefab de sangre (VFX Graph). Se instancia con offset y rotación configurables.")]
    [SerializeField] private GameObject bloodVFX;
    [Tooltip("Desplazamiento vertical respecto al pivote (0,0,0) del enemigo. Negativo = hacia abajo.")]
    [SerializeField] private float bloodVFXYOffset = -1.8f;
    [Tooltip("Si está activo, orienta el VFX en la dirección del disparo (hacia donde iba la bala, lejos del arma). Si no, usa la rotación fija de abajo.")]
    [SerializeField] private bool bloodVFXFaceHitDirection = false;
    [Tooltip("Rotación (euler) del prefab. Si 'Face Hit Direction' está activo, se suma como offset.")]
    [SerializeField] private Vector3 bloodVFXEulerRotation;
    [SerializeField] private float bloodVFXLifetime = 5f;

    [Header("Cabeza")]
    [Tooltip("GameObject de la cabeza skinneada (la separada en 3ds Max). Se oculta al recibir headshot.")]
    [SerializeField] private GameObject attachedHead;
    [Tooltip("Objetos extra que deben ocultarse junto con la cabeza (orejas, pelo, gafas, etc.).")]
    [SerializeField] private GameObject[] attachedHeadExtras;
    [Tooltip("Hueso de la cabeza — referencia para spawnear el VFX y la cabeza física en su posición.")]
    [SerializeField] private Transform headBone;
    [Tooltip("Prefab de la cabeza física: mesh + Rigidbody + Collider.")]
    [SerializeField] private GameObject detachedHeadPrefab;
    [SerializeField] private float headLaunchForce = 6f;
    [SerializeField] private float headExtraTorque = 8f;
    [SerializeField] private float detachedHeadLifetime = 8f;

    [Header("Ragdoll")]
    [SerializeField] private RagdollController ragdoll;
    [Tooltip("Multiplicador aplicado a la fuerza del disparo cuando se traduce a impulso del ragdoll.")]
    [SerializeField] private float hitImpulseMultiplier = 1f;

    [Header("Bodyshot — desaparición del cuerpo entero")]
    [SerializeField] private float bodyDespawnDelay = 6f;

    [Header("Empuje en área (afecta a otros, no a sí mismo)")]
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
        // PARCHE (Bug fix sangre headshot): priorizamos Head. Si ya registramos un impacto en la
        // cabeza durante la vida del enemigo, no permitimos que un perdigón posterior del shotgun
        // que impacte en otra zona lo "sobrescriba". Así, basta con que UN solo perdigón del disparo
        // dé en la cabeza para que muera por headshot y se spawnee el VFX de sangre.
        // hitPoint / hitDirection / hitForce se siguen actualizando para que el ragdoll reaccione
        // al último impacto físico real (lo único que conservamos es el "tipo" como Head).
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

        Quaternion rot;
        if (bloodVFXFaceHitDirection && lastHitDirection.sqrMagnitude > 0.0001f)
            rot = Quaternion.LookRotation(lastHitDirection) * Quaternion.Euler(bloodVFXEulerRotation);
        else
            rot = Quaternion.Euler(bloodVFXEulerRotation);

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