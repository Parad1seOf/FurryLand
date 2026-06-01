// Sistema de disparo por raycast. Lee el estado de PlayerController para calcular la dispersión,
// despacha daño a IDamageable, consulta BodyPart para multiplicadores y llama a AudioManager para el audio.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    public Transform muzzlePoint;

    [Header("Damage")]
    public int damage = 25;
    public float range = 100f;
    public LayerMask hitMask = ~0;

    [Header("Bullet Distance")]
    public float bulletDistance;

    [Header("Impact")]
    public float bulletImpactForce = 15f;

    [Header("Fire Rate")]
    public float timeBetweenShots = 0.1f;
    public bool allowHoldToFire = true;

    [Header("Spread")]
    public float spread = 0.00f;
    private float currentSpread;

    public bool guaranteeCenterPellet = true;

    [Header("Shotgun Pellets")]
    public int minPellets = 6;
    public int maxPellets = 10;

    [Header("Magazine")]
    public int magazineCapacity = 30;
    public int totalMagazines = 3;
    public float reloadTime = 1.0f;

    [Header("VFX")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;

    [Header("Animation")]
    public Animation weaponAnimation;
    public AnimationClip idleClip;
    public AnimationClip shootClip;
    public AnimationClip reloadClip;
    public float shootBlendIn = 0.05f;
    public float shootBlendOut = 0.1f;

    #endregion

    #region Private State

    private int bulletsLeft;
    private int magazinesLeft;
    private bool readyToShoot = true;
    private bool isReloading = false;

    private Coroutine resetShotCoroutine;
    private Coroutine reloadCoroutine;

    private struct BulletGizmo
    {
        public Vector3 start;
        public Vector3 end;
    }

    private readonly List<BulletGizmo> bulletGizmos = new List<BulletGizmo>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        bulletsLeft = magazineCapacity;
        magazinesLeft = totalMagazines;
        readyToShoot = true;

        bulletDistance = range;

        RegisterClip(idleClip);
        RegisterClip(shootClip);
        RegisterClip(reloadClip);

        PlayAnimation(idleClip);
    }

    #endregion

    private void RegisterClip(AnimationClip clip)
    {
        if (weaponAnimation == null || clip == null) return;

        weaponAnimation.RemoveClip(clip.name);
        weaponAnimation.AddClip(clip, clip.name);
    }

    #region Shooting

    public bool TryShoot(Vector3 origin, Vector3 direction)
    {
        if (Time.timeScale == 0f) return false;

        if (readyToShoot && !isReloading)
        {
            if (bulletsLeft > 0)
            {
                Shoot(origin, direction);
                return true;
            }
            else
            {
                Reload();
            }
        }
        return false;
    }

    private void Shoot(Vector3 origin, Vector3 direction)
    {
        readyToShoot = false;
        bulletsLeft--;

        PlayShootAnimation();
        SpawnMuzzleFlash();

        int pellets = UnityEngine.Random.Range(minPellets, maxPellets + 1);
        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < pellets; i++)
        {
            float x, y;

            if (guaranteeCenterPellet && i == 0)
            {
                x = 0f;
                y = 0f;
            }
            else
            {
                x = UnityEngine.Random.Range(-spread, spread);
                y = UnityEngine.Random.Range(-spread, spread);
            }

            Vector3 spreadDirection = rotation * new Vector3(x, y, 1f);
            spreadDirection.Normalize();

            try
            {
                RaycastHit[] hits = Physics.RaycastAll(
                    origin,
                    spreadDirection,
                    bulletDistance,
                    hitMask,
                    QueryTriggerInteraction.Ignore
                );

                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                bool hitPlayerOnce = false;

                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("MAP"))
                    {
                        bulletGizmos.Add(new BulletGizmo
                        {
                            start = origin,
                            end = hit.point
                        });

                        ProcessHit(hit, spreadDirection);
                        SpawnBulletHole(hit);
                        break;
                    }

                    if (hit.collider.CompareTag("Player"))
                    {
                        if (hitPlayerOnce) continue;
                        hitPlayerOnce = true;
                    }

                    bulletGizmos.Add(new BulletGizmo
                    {
                        start = origin,
                        end = hit.point
                    });

                    ProcessHit(hit, spreadDirection);
                }

                if (hits.Length == 0)
                {
                    bulletGizmos.Add(new BulletGizmo
                    {
                        start = origin,
                        end = origin + spreadDirection * bulletDistance
                    });
                }
            }
            catch (Exception e)
            {
            }
        }

        ResetSpread();

        if (resetShotCoroutine != null)
            StopCoroutine(resetShotCoroutine);

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
            float finalDamage = damage;

            BodyPart bodyPart = hit.collider.GetComponent<BodyPart>();

            if (bodyPart != null)
                finalDamage *= bodyPart.damageMultiplier;

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

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

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
        if (resetShotCoroutine != null)
        {
            StopCoroutine(resetShotCoroutine);
            resetShotCoroutine = null;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        bulletsLeft = magazineCapacity;
        magazinesLeft = totalMagazines;
        isReloading = false;
        readyToShoot = true;

        PlayAnimation(idleClip);
    }

    public void AddMagazines(int amount)
    {
        magazinesLeft += amount;
    }

    public int BulletsLeft => bulletsLeft;
    public int MagazineCapacity => magazineCapacity;
    public int MagazinesLeft => magazinesLeft;
    public bool InfiniteAmmo => totalMagazines == -1;

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

        if (weaponAnimation.GetClip(clip.name) == null)
        {
            return;
        }

        weaponAnimation.CrossFadeQueued(clip.name, fadeTime);
    }

    private void PlayShootAnimation()
    {
        PlayAnimation(shootClip, shootBlendIn);
        QueueAnimation(idleClip, shootBlendOut);
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        foreach (var g in bulletGizmos)
        {
            Gizmos.DrawLine(g.start, g.end);
            Gizmos.DrawSphere(g.end, 0.02f);
        }
    }
#endif
}