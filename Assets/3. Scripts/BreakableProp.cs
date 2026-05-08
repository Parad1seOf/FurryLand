using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BreakableProp : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 25f;

    [Header("Rotura")]
    [SerializeField] private GameObject brokenPrefab;
    [SerializeField] private float fragmentLifetime = 5f;
    [SerializeField] private float fragmentScatter = 2f;

    [Header("VFX / SFX (opcional)")]
    [SerializeField] private GameObject breakVFXPrefab;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    [Header("Reaccion al impacto")]
    [SerializeField] private float hitImpulse = 3f;
    [SerializeField, Range(0f, 1f)] private float hitRandomness = 0.2f;
    [SerializeField, Range(0f, 1f)] private float hitUpwardBias = 0.3f;

    private Rigidbody rb;
    private Transform playerTransform;
    private float currentHealth;
    private bool broken = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public void TakeDamage(float amount)
    {
        if (broken) return;

        currentHealth -= amount;
        ApplyHitImpulse();

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, volume);

        if (currentHealth <= 0f)
            Break();
    }

    private void ApplyHitImpulse()
    {
        if (rb == null || rb.isKinematic || hitImpulse <= 0f) return;

        Vector3 dir;

        if (playerTransform != null)
            dir = (transform.position - playerTransform.position).normalized;
        else
            dir = Random.insideUnitSphere.normalized;

        dir += new Vector3(
            Random.Range(-hitRandomness, hitRandomness),
            hitUpwardBias + Random.Range(0f, hitRandomness),
            Random.Range(-hitRandomness, hitRandomness)
        );
        dir.Normalize();

        rb.AddForce(dir * hitImpulse, ForceMode.Impulse);
    }

    private void Break()
    {
        broken = true;

        if (brokenPrefab != null)
        {
            GameObject fragments = Instantiate(brokenPrefab, transform.position, transform.rotation);

            Rigidbody[] fragmentRBs = fragments.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody fragRB in fragmentRBs)
            {
                if (rb != null) fragRB.linearVelocity = rb.linearVelocity;
                fragRB.AddForce(Random.insideUnitSphere * fragmentScatter, ForceMode.Impulse);
            }

            Destroy(fragments, fragmentLifetime);
        }

        if (breakVFXPrefab != null)
        {
            GameObject vfx = Instantiate(breakVFXPrefab, transform.position, transform.rotation);
            Destroy(vfx, 3f);
        }

        if (breakSound != null)
            AudioSource.PlayClipAtPoint(breakSound, transform.position, volume);

        Destroy(gameObject);
    }
}