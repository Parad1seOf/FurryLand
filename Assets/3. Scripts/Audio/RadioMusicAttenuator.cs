using UnityEngine;

public class RadioMusicBalancer : MonoBehaviour
{
    [Header("Audio de fondo que será atenuado")]
    public AudioSource backgroundMusic;

    [Header("Volumen máximo del audio de fondo")]
    [Range(0f, 1f)]
    public float maxBackgroundVolume = 0.6f;

    private AudioSource radioAudio;
    private Transform player;

    private void Start()
    {
        radioAudio = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        if (backgroundMusic != null)
            backgroundMusic.volume = maxBackgroundVolume;
    }

    private void Update()
    {
        if (backgroundMusic == null)
            return;

        if (player == null || radioAudio == null)
        {
            backgroundMusic.volume = maxBackgroundVolume;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        float minDistance = radioAudio.minDistance;
        float maxDistance = radioAudio.maxDistance;

        float targetVolume;

        if (distance <= minDistance)
        {
            targetVolume = 0f;
        }
        else if (distance >= maxDistance)
        {
            targetVolume = maxBackgroundVolume;
        }
        else
        {
            float t = (distance - minDistance) / (maxDistance - minDistance);

            targetVolume = Mathf.Lerp(0f, maxBackgroundVolume, t);
        }

        backgroundMusic.volume = targetVolume;
    }

    private void OnDestroy()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = maxBackgroundVolume;
        }
    }

    private void OnDrawGizmosSelected()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, audioSource.minDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, audioSource.maxDistance);
    }
}