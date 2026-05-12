// Sistema de disparo por raycast. Lee el estado de PlayerController para calcular la dispersiÃ³n,
// despacha daÃ±o a IDamageable, consulta BodyPart para multiplicadores y llama a AudioManager para el audio.
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

    [Header("Fire Rate")]
    public float timeBetweenShots = 0.1f;
    public bool  allowHoldToFire  = true;

    [Header("Spread")]
    public float spread = 0.00f;
    private float currentSpread;

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
        bulletsLeft--;


        PlayShootAnimation();
        SpawnMuzzleFlash();

        float   x         = UnityEngine.Random.Range(-spread, spread);
        float   y         = UnityEngine.Random.Range(-spread, spread);

        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 spreadDirection = rotation * new Vector3(x, y, 1f);
        spreadDirection.Normalize();

        ResetSpread();

        try
        {
            if (Physics.Raycast(origin, spreadDirection, out RaycastHit hit, range,
                            ~0, QueryTriggerInteraction.Ignore))
            {
                ProcessHit(hit);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        

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

    private void ProcessHit(RaycastHit hit)
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