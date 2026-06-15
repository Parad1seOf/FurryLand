using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioMusicBalancer : MonoBehaviour
{
    [Header("Audio de fondo que será atenuado")]
    public AudioSource backgroundMusic;

    [Header("Volumen máximo del audio de fondo")]
    [Range(0f, 1f)]
    public float maxBackgroundVolume = 0.6f;

    [Header("Distancias de influencia")]
    public float innerRadius = 5f;
    public float outerRadius = 20f;

    private Transform player;
    private bool playerInside = false;
    private SphereCollider triggerCollider;

    private void Awake()
    {
        // Crea (o reutiliza) un SphereCollider como Trigger
        triggerCollider = GetComponent<SphereCollider>();

        if (triggerCollider == null)
            triggerCollider = gameObject.AddComponent<SphereCollider>();

        triggerCollider.isTrigger = true;
        triggerCollider.radius = outerRadius;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        if (backgroundMusic != null)
            backgroundMusic.volume = maxBackgroundVolume;
    }

    private void Update()
    {
        // Mantener el radio actualizado si lo cambias desde el Inspector
        if (triggerCollider.radius != outerRadius)
            triggerCollider.radius = outerRadius;

        if (!playerInside)
            return;

        if (backgroundMusic == null || player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        float targetVolume;

        if (distance <= innerRadius)
        {
            targetVolume = 0f;
        }
        else if (distance >= outerRadius)
        {
            targetVolume = maxBackgroundVolume;
        }
        else
        {
            float t = (distance - innerRadius) / (outerRadius - innerRadius);
            targetVolume = Mathf.Lerp(0f, maxBackgroundVolume, t);
        }

        backgroundMusic.volume = targetVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (backgroundMusic != null)
            backgroundMusic.volume = maxBackgroundVolume;
    }

    private void OnDestroy()
    {
        if (backgroundMusic != null)
            backgroundMusic.volume = maxBackgroundVolume;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (outerRadius < innerRadius)
            outerRadius = innerRadius;

        SphereCollider col = GetComponent<SphereCollider>();

        if (col != null)
        {
            col.isTrigger = true;
            col.radius = outerRadius;
        }
    }
#endif
}