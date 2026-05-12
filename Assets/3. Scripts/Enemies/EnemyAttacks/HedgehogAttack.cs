using UnityEngine;

public class HedgehogAttack : EnemyAttack
{
    
    [SerializeField] private float distance;
    [SerializeField] private float cooldown;
    private float cooldownTimer;

    public override void Attack(Vector3 targetDirection)
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            HealthSystem health = hit.collider.GetComponent<HealthSystem>();

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
