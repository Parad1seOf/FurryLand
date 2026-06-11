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

    [SerializeField] private Color aimingColor = Color.blue;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color shootColor1 = Color.black;
    [SerializeField] private Color shootColor2 = Color.white;

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

            beam.startColor = aimingColor;
            beam.endColor = aimingColor;

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
                    beam.startColor = aimingColor;
                    beam.endColor = aimingColor;
                }
            }
            else
            {
                timer -= Time.deltaTime;

                if (!warningActive && timer <= 1f)
                {
                    warningActive = true;
                    beam.startColor = warningColor;
                    beam.endColor = warningColor;
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

        // NEGRO
        beam.startColor = shootColor1;
        beam.endColor = shootColor1;
        yield return new WaitForSeconds(0.03f);

        // BLANCO
        beam.startColor = shootColor2;
        beam.endColor = shootColor2;
        yield return new WaitForSeconds(0.03f);

        // ROJO
        beam.startColor = warningColor;
        beam.endColor = warningColor;
        yield return new WaitForSeconds(0.05f);

        if (sniperAnimation != null)
            sniperAnimation.Shoot();

        Vector3 direction = (target - origin.position).normalized;
        if (Physics.Raycast(origin.position, direction, out RaycastHit hit, Mathf.Infinity, beamCollisionMask))
        {
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //Esto da error porque he quitado la variable de daño de la clase abstracta.
                //Esta clase no deberia de contener el daño aqui porque hace de adaptador al arma
                //que es quien tiene que tener el daño.
                //De hecho, se esta llamando abajo al disparo, asi que hace daño 2 veces (mal).
                //Comento la linea
                //playerHealth.TakeDamage(damage, "Sniper");
            }
        }

        //Aqui se esta llamando al disparo
        gun.TryShoot(origin.position, target - origin.position);
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