// Sistema de disparo por raycast. Lee el estado de PlayerController para calcular la dispersión,
// despacha daño a IDamageable, consulta BodyPart para multiplicadores y llama a AudioManager para el audio.
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    public AudioManager     audioManager;
    public PlayerController playerController;
    public Camera           fpsCam;
    public Transform        muzzlePoint;

    [Header("Damage")]
    public int   damage = 25;
    public float range  = 100f;

    [Header("Fire Rate")]
    public float timeBetweenShots = 0.1f;
    public bool  allowHoldToFire  = true;

    [Header("Spread")]
    public float idleSpread = 0.00f;
    public float walkSpread = 0.02f;
    public float runSpread  = 0.05f;

    [Header("Magazine")]
    public int   magazineCapacity = 30;
    public int   totalMagazines   = 3;
    public float reloadTime       = 1.8f;

    [Header("Recoil")]
    public CharacterController characterController;
    public float recoilForce = 25f;
    private Vector3 impactVelocity;

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

    private void Update()
    {
        HandleInput();

        if (impactVelocity.magnitude > 0.2f)
        {
            characterController.Move(impactVelocity * Time.deltaTime);
            impactVelocity = Vector3.Lerp(impactVelocity, Vector3.zero, 5f * Time.deltaTime);
        }
    }

    #endregion

    #region Input

    private void HandleInput()
    {
        if (Time.timeScale == 0f) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        bool trigger = allowHoldToFire
            ? Input.GetKey(KeyCode.Mouse0)
            : Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsLeft < magazineCapacity)
            StartReload();

        if (readyToShoot && trigger && !isReloading)
        {
            if (bulletsLeft > 0)
                Shoot();
            else
                StartReload();
        }
    }

    #endregion

    #region Shooting

    private void Shoot()
    {
        readyToShoot = false;
        bulletsLeft--;

        PlayShootAnimation();
        SpawnMuzzleFlash();
        audioManager?.Shooting();

        float spread = idleSpread;
        if      (playerController != null && playerController.IsRunning) spread = runSpread;
        else if (playerController != null && playerController.IsWalking) spread = walkSpread;

        float   x         = Random.Range(-spread, spread);
        float   y         = Random.Range(-spread, spread);
        Vector3 direction = fpsCam.transform.forward
                          + fpsCam.transform.right * x
                          + fpsCam.transform.up    * y;

        if (characterController != null)
        {
            if (!characterController.isGrounded)
            {
                impactVelocity += -direction.normalized * recoilForce;
            }
        }


        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit hit, range,
                            ~0, QueryTriggerInteraction.Ignore))
        {
            ProcessHit(hit);
        }

        if (resetShotCoroutine != null) StopCoroutine(resetShotCoroutine);
        resetShotCoroutine = StartCoroutine(ResetShotRoutine());




        AlertSystem.Instance.TriggerAlert();
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
                if (bodyPart.partType == BodyPartType.Head)
                    audioManager?.Headshot();
                else
                    audioManager?.BodyHit();
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

    private void StartReload()
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
}