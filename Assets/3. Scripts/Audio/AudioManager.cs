// Script sing de audio. PlayerController y GunSystem llaman sus métodos...
// para reproducir sonidos de disparo etcc
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance { get; private set; }

    #endregion

    #region Inspector Fields

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource stepsSource;

    [Header("Clips — Jugador")]
    public AudioClip shootClip;
    public AudioClip playerHitClip;
    public AudioClip footstepsClip;

    [Header("Clips — Combate")]
    public AudioClip headShotClip;
    public AudioClip bodyHitClip;
    public AudioClip meleeSwipeClip;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Public API

    public void Shooting()    => PlayOneShot(sfxSource, shootClip);
    public void PlayerHit()   => PlayOneShot(sfxSource, playerHitClip);
    public void Headshot()    => PlayOneShot(sfxSource, headShotClip);
    public void BodyHit()     => PlayOneShot(sfxSource, bodyHitClip);
    public void MeleeSwipe()  => PlayOneShot(sfxSource, meleeSwipeClip);

    public void Walking()
    {
        if (stepsSource == null || footstepsClip == null) return;
        if (stepsSource.isPlaying) return;
        stepsSource.clip = footstepsClip;
        stepsSource.loop = true;
        stepsSource.Play();
    }

    public void StopWalking()
    {
        if (stepsSource != null && stepsSource.isPlaying)
            stepsSource.Stop();
    }

    #endregion

    #region Helpers

    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.PlayOneShot(clip);
    }

    #endregion
}