// Sistema de disparo por raycast. Lee el estado de PlayerController para calcular la dispersión,
// despacha daño a IDamageable, consulta BodyPart para multiplicadores y llama a AudioManager para el audio.
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    public Transform        muzzlePoint;
    [Header("Damage")]
    public int   damage = 25;
    public float range  = 100f;
    public LayerMask hitMask = ~0;   // <— nuevo

    [Header("Impact")]
    [Tooltip("Fuerza con la que el disparo empuja al ragdoll cuando mata a un enemigo. Subir para efecto más cinematográfico.")]
    public float bulletImpactForce = 15f;

    [Header("Fire Rate")]
    public float timeBetweenShots = 0.1f;
    public bool  allowHoldToFire  = true;

    [Header("Spread")]
    public float spread = 0.00f;
    private float currentSpread;

    [Header("Shotgun Pellets")]
    [Tooltip("Mínimo de perdigones por disparo (inclusive).")]
    public int minPellets = 6;
    [Tooltip("Máximo de perdigones por disparo (inclusive).")]
    public int maxPellets = 10;

    [Header("Magazine")]
    public int   magazineCapacity = 30;
    public int   totalMagazines   = 3;
    public float reloadTime       = 1.8f;

    [Header("VFX")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;


    [Header("Animation")]
    public Animation     weaponAnimation;
    public AnimationClip idleClip;
    public AnimationClip shootClip;
    public AnimationClip reloadClip;
    public float         shootBlendIn  = 0.05f;
    public float         shootBlendOut = 0.1f;

    #endregion

    #region Private State

    private int  bulletsLeft;
    private int  magazinesLeft;
    private bool readyToShoot = true;
    private bool isReloading  = false;

    private Coroutine resetShotCoroutine;
    private Coroutine reloadCoroutine;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        bulletsLeft   = magazineCapacity;
        magazinesLeft = totalMagazines;
        readyToShoot  = true;
        PlayAnimation(idleClip);
    }

    #endregion

    #region Shooting

    public bool TryShoot(Vector3 origin, Vector3 direction)
    {
        if (Time.timeScale == 0f) return false;
        
        if (readyToShoot  && !isReloading)
        {
            if (bulletsLeft > 0)
            {
                Shoot(origin, direction);
                return true;
            }
            else
                Reload();
        }
        return false;
    }

    private void Shoot(Vector3 origin, Vector3 direction)
    {
        readyToShoot = false;
        bulletsLeft--;   // sólo 1 bala consumida por disparo, aunque salgan N perdigones

        PlayShootAnimation();
        SpawnMuzzleFlash();

        // Número de perdigones aleatorio dentro del margen
        int pellets = UnityEngine.Random.Range(minPellets, maxPellets + 1);

        Quaternion rotation = Quaternion.LookRotation(direction);

        bool hasDamagedPlayerThisShot = false;  //evita golpear más de una vez al player

        for (int i = 0; i < pellets; i++)
        {
            // Dispersión cuadrada: X e Y independientes => coincide con el hitmarker cuadrado
            float x = UnityEngine.Random.Range(-spread, spread);
            float y = UnityEngine.Random.Range(-spread, spread);

            Vector3 spreadDirection = rotation * new Vector3(x, y, 1f);
            spreadDirection.Normalize();

            try
            {
                if (Physics.Raycast(origin, spreadDirection, out RaycastHit hit, range,
                                    hitMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        if (hasDamagedPlayerThisShot) continue;

                        hasDamagedPlayerThisShot = true;
                    }

                    Debug.Log($"PELLET HIT: {hit.collider.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    ProcessHit(hit, spreadDirection);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        ResetSpread();   // se llama una sola vez, al final

        if (resetShotCoroutine != null) StopCoroutine(resetShotCoroutine);
        resetShotCoroutine = StartCoroutine(ResetShotRoutine());
    }

    public void IncreaseSpread(float amount)
    {
        currentSpread = spread + amount;
    }

    private void ResetSpread()
    {
        currentSpread = spread;
    }

    private void ProcessHit(RaycastHit hit, Vector3 shotDirection)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            float    finalDamage = damage;
            BodyPart bodyPart    = hit.collider.GetComponent<BodyPart>();

            if (bodyPart != null)
            {
                finalDamage *= bodyPart.damageMultiplier;
                /*if (bodyPart.partType == BodyPartType.Head)
                    audioManager?.Headshot();
                else
                    audioManager?.BodyHit();*/
            }

            // Notificar al DeathExplosion del enemigo qué zona se ha impactado,
            // dónde, en qué dirección y con cuánta fuerza, para que el ragdoll reaccione bien.
            DeathExplosion de = hit.collider.GetComponentInParent<DeathExplosion>();
            if (de != null)
            {
                BodyPartType partType = bodyPart != null ? bodyPart.partType : BodyPartType.Default;
                de.NotifyHit(partType, hit.point, shotDirection, bulletImpactForce);
            }

            damageable.TakeDamage(finalDamage);
        }
        else
        {
            SpawnBulletHole(hit);
        }
    }

    private IEnumerator ResetShotRoutine()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToShoot = true;
        resetShotCoroutine = null;
    }

    #endregion

    #region Reload

    public void TryReload()
    {
        if (!isReloading && bulletsLeft < magazineCapacity)
            Reload();
    }

    private void Reload()
    {
        if (totalMagazines != -1 && magazinesLeft <= 0) return;
        if (isReloading) return;

        isReloading = true;
        PlayAnimation(reloadClip);
        QueueAnimation(idleClip);

        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);

        if (totalMagazines != -1)
            magazinesLeft--;

        bulletsLeft = magazineCapacity;
        isReloading = false;
        reloadCoroutine = null;
    }

    public void ResetGun()
    {
        if (resetShotCoroutine != null) { StopCoroutine(resetShotCoroutine); resetShotCoroutine = null; }
        if (reloadCoroutine    != null) { StopCoroutine(reloadCoroutine);    reloadCoroutine    = null; }

        bulletsLeft   = magazineCapacity;
        magazinesLeft = totalMagazines;
        isReloading   = false;
        readyToShoot  = true;
        PlayAnimation(idleClip);
    }

    public void AddMagazines(int amount)
    {
        magazinesLeft += amount;
    }

    public int  BulletsLeft      => bulletsLeft;
    public int  MagazineCapacity => magazineCapacity;
    public int  MagazinesLeft    => magazinesLeft;
    public bool InfiniteAmmo     => totalMagazines == -1;

    #endregion

    #region VFX

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;
        GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position,
                                       Quaternion.identity, muzzlePoint);
        Destroy(flash, 0.05f);
    }

    private void SpawnBulletHole(RaycastHit hit)
    {
        if (bulletHolePrefab == null) return;
        GameObject hole = Instantiate(bulletHolePrefab, hit.point,
                                      Quaternion.LookRotation(hit.normal));
        Destroy(hole, 5f);
    }

    #endregion

    #region Animation

    private void PlayAnimation(AnimationClip clip, float fadeTime = 0.1f)
    {
        if (weaponAnimation == null || clip == null) return;
        weaponAnimation.CrossFade(clip.name, fadeTime);
    }

    private void QueueAnimation(AnimationClip clip, float fadeTime = 0.1f)
    {
        if (weaponAnimation == null || clip == null) return;
        weaponAnimation.CrossFadeQueued(clip.name, fadeTime);
    }

    private void PlayShootAnimation()
    {
        PlayAnimation(shootClip, shootBlendIn);
        QueueAnimation(idleClip, shootBlendOut);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position;
    }
#endif
}