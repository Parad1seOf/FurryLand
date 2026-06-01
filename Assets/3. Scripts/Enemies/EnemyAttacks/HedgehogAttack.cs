using UnityEngine;


//Esta clase esta fatal, hay que arreglarlo
public class HedgehogAttack : EnemyAttack
{
    
    [SerializeField] private float distance = 2f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform origin;

    [SerializeField] private Animator animator;

    [SerializeField] private float timeToHit = 0.15f;
    private float hitTimer;
    private float cooldownTimer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public override void Attack(Vector3 targetDirection)
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        isAttacking = true;

        cooldownTimer = cooldown;
        hitTimer = timeToHit;
    }

    public void RealAttack()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin.position, direction, out hit, distance))
        {
            HealthSystem health = hit.collider.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        isAttacking = false;
    }

    public void Update()
    {
        if (cooldownTimer >= 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isAttacking) return;

        if (hitTimer >= 0)
        {
            hitTimer -= Time.deltaTime;
        }

        if (hitTimer <= 0)
        {
            RealAttack();
        }
    }
}
