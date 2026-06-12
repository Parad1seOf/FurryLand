using UnityEngine;
using System.Collections;

public class SlothAttack : EnemyAttack
{
    [SerializeField] private GunSystem gun;
    [SerializeField] private Transform origin;
    [SerializeField] private float distance = 50f;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float timeToShoot = 2;

    // NUEVO: layers con los que sí puede colisionar el raycast
    [SerializeField] private LayerMask beamCollisionMask = ~0;

    [SerializeField] private Material aimingMaterial;
    [SerializeField] private Material warningMaterial;
    [SerializeField] private Material shootMaterial1;
    [SerializeField] private Material shootMaterial2;

    [SerializeField] private SniperAnimationController sniperAnimation;

    private Vector3 target;
    private Vector3 follow;
    public float timer;

    private LineRenderer beam;

    private bool warningActive;
    private bool shootingFlash;

    public void Awake()
    {
        if (sniperAnimation == null)
            sniperAnimation = GetComponent<SniperAnimationController>();
    }

    public void Start()
    {
        beam = GetComponent<LineRenderer>();
        beam.positionCount = 2;
        beam.enabled = false;
    }

    public override void Attack(Vector3 target)
    {
        if (!isAttacking)
        {
            follow = target;
            isAttacking = true;
            timer = timeToShoot;

            warningActive = false;
            shootingFlash = false;

            beam.material = aimingMaterial;

            beam.enabled = true;

            if (sniperAnimation != null)
                sniperAnimation.SetAiming(true);
        }

        this.target = target;
    }

    public void Update()
    {
        if (!isAttacking) return;
        if (shootingFlash) return;

        follow = Vector3.MoveTowards(follow, target, followSpeed * Time.deltaTime);
        Beam();

        if (timer < 0)
        {
            StartCoroutine(ShootFlash());
        }
    }

    private void Beam()
    {
        Vector3 direction = follow - origin.position;
        direction.Normalize();

        beam.SetPosition(0, origin.position);

        // AQUÍ se aplica el LayerMask
        if (Physics.Raycast(origin.position, direction, out RaycastHit hit, Mathf.Infinity, beamCollisionMask))
        {
            beam.SetPosition(1, hit.point);

            PlayerTarget player = hit.collider.GetComponent<PlayerTarget>();

            if (player == null)
            {
                timer = timeToShoot;

                if (warningActive)
                {
                    warningActive = false;
                    beam.material = aimingMaterial;
                }
            }
            else
            {
                timer -= Time.deltaTime;

                if (!warningActive && timer <= 1f)
                {
                    warningActive = true;
                    beam.material = warningMaterial;
                }
            }
        }
        else
        {
            beam.SetPosition(1, origin.position + direction * 100);
        }
    }

    private IEnumerator ShootFlash()
    {
        shootingFlash = true;

        // Primer material
        beam.material = shootMaterial1;
        yield return new WaitForSeconds(0.03f);

        // Segundo material
        beam.material = shootMaterial2;
        yield return new WaitForSeconds(0.03f);

        // Material de advertencia
        beam.material = warningMaterial;
        yield return new WaitForSeconds(0.05f);

        if (sniperAnimation != null)
            sniperAnimation.Shoot();

        if (gun != null)
        {
            gun.TryShoot(origin.position, target - origin.position, true);
        }

        AudioManager.Instance.SniperShoot(origin.position);

        EndAttack();
    }

    public override void EndAttack()
    {
        base.EndAttack();
        beam.enabled = false;

        if (sniperAnimation != null)
            sniperAnimation.SetAiming(false);
    }
}