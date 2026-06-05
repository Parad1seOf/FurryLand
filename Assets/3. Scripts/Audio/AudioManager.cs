// Script sing de audio. PlayerController y GunSystem llaman sus métodos...
// para reproducir sonidos de disparo etcc
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance { get; private set; }

    public static event System.Action OnHeadshotPerformed;

    #endregion

    #region Inspector Fields

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource stepsSource;
    public AudioSource musicSource;

    [Header("Clips — Jugador")]
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public AudioClip playerHitClip;
    public AudioClip footstepsClip;

    [Header("Clips — Combate")]
    public AudioClip headShotClip;
    public AudioClip bodyHitClip;
    public AudioClip meleeSwipeClip;

    [Header("Clips — Victoria")]
    public AudioClip victoryMusicClip;
    public AudioClip defeatMusicClip;

    [Header("Clips — Ambiente")]
    public AudioClip birdsClip;

    [Header("Clips — Enemigos")]
    public AudioClip suspicion;
    public AudioClip alert;
    public AudioClip rabbitStepClip;
    public AudioClip bigRabbitStepClip;
    public AudioClip rabbitDeathClip;
    public AudioClip bigRabbitDeathClip;
    public AudioClip rabbitAttackClip;
    public AudioClip sniperShootClip;

    [Header("Clips - Interactuables")]
    public AudioClip pickUpWoodClip;
    public AudioClip blockDoorClip;

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
    public void Reloading() => PlayOneShot(sfxSource, reloadClip);
    public void PlayerHit()   => PlayOneShot(sfxSource, playerHitClip);
    public void BodyHit()     => PlayOneShot(sfxSource, bodyHitClip);
    public void MeleeSwipe()  => PlayOneShot(sfxSource, meleeSwipeClip);

    public void Headshot()
    {
        PlayOneShot(sfxSource, headShotClip);
        OnHeadshotPerformed?.Invoke();
    }

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

    public void PlayBirdsAmbience()
    {
        if (musicSource == null || birdsClip == null) return;

        musicSource.clip = birdsClip;
        musicSource.loop = true;
        musicSource.ignoreListenerPause = false;
        musicSource.Play();
    }

    public void StopBirdsAmbience()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    public void Suspicion(Vector3 position) => PlayOneShot(suspicion, position);
    public void Alert(Vector3 position) => PlayOneShot(alert, position);
    public void RabbitStep(Vector3 position) => PlayOneShot(rabbitStepClip, position);
    public void BigRabbitStep(Vector3 position) => PlayOneShot(bigRabbitStepClip, position);
    public void RabbitDeath(Vector3 position) => PlayOneShot(rabbitDeathClip, position);
    public void BigRabbitDeath(Vector3 position) => PlayOneShot(bigRabbitDeathClip, position);
    public void RabbitAttack(Vector3 position) => PlayOneShot(rabbitAttackClip, position);
    public void SniperShoot(Vector3 position) => PlayOneShot(sniperShootClip, position);
    public void PickUpWood(Vector3 position) => PlayOneShot(pickUpWoodClip, position);
    public void BlockDoor(Vector3 position) => PlayOneShot(blockDoorClip, position);


    #endregion

    #region Helpers

    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.PlayOneShot(clip);
    }

    private void PlayOneShot(AudioClip clip, Vector3 position)
    {
        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.position = position;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.PlayOneShot(clip);
        Destroy(audioSource, clip.length);
    }

    #endregion
}