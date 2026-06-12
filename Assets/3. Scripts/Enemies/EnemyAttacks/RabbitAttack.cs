using UnityEngine;


//Esta clase esta fatal, hay que arreglarlo
public class RabbitAttack : EnemyAttack, IAnimatedAttack
{
    [SerializeField] private float damage = 10;
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
        Vector3 direction = transform.forward;
        PlayerController playerTarget = Object.FindFirstObjectByType<PlayerController>();

        if (playerTarget != null)
        {
            Vector3 targetCenter = playerTarget.transform.position + Vector3.up * 1f;
            direction = (targetCenter - origin.position).normalized;
        }

        RaycastHit hit;
       
        if (Physics.Raycast(origin.position, direction, out hit, distance))
        {
            PlayerHealth health = hit.collider.GetComponentInParent<PlayerHealth>();

            if (health != null)
                health.TakeDamage(damage, "Rabbit");
        }

        AudioManager.Instance?.RabbitAttack(transform.position);

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
