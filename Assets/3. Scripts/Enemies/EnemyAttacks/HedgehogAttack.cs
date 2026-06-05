using UnityEngine;


//Esta clase esta fatal, hay que arreglarlo
public class HedgehogAttack : EnemyAttack, IAnimatedAttack
{
    
    [SerializeField] private float distance = 2f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform origin;

    [SerializeField] private Animator animator;

    [SerializeField] private float timeToHit = 0.15f;
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
    }

    public void RealAttack()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin.position, direction, out hit, distance))
        {
            PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();

            if (health != null)
                health.TakeDamage(damage, "Rabbit");
        }

        /*if (Physics.Raycast(origin.position, direction, out hit, distance))
        {
            HealthSystem health = hit.collider.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }*/

        AudioManager.Instance.RabbitAttack(transform.position);

        isAttacking = false;
    }

    public void Update()
    {
        if (cooldownTimer >= 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void AnimatedAttack()
    {
        RealAttack();
    }
}
