using UnityEngine;

public class HedgehogAttack : EnemyAttack
{
    
    [SerializeField] private float distance = 2f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform origin;

    [SerializeField] private Animator animator;

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

        cooldownTimer = cooldown;
    }

    public void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
