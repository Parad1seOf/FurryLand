using Unity.VisualScripting;
using UnityEngine;

public class RabbitAttack : EnemyAttack
{
    private enum State
    {
        PREPARING,
        JUMPING,
        NOTHING
    }

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float timeBeforeJump = 0.5f;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float height = 1f;

    private State state;
    private float timer;
    private Vector3 direction;
    private float groundCheckOffset;
    private float groundCheckTimer;

    public override void Attack(Vector3 targetDirection)
    {
        isAttacking = true;
        direction = targetDirection;
        GetComponent<HealthSystem>().OnDeath += EndAttack;
    }

    public void Update()
    {
        if (!isAttacking) return;

        switch (state)
        {
            case State.PREPARING:
                PrepareJump();
                break;

            case State.JUMPING:
                Jumping();
                break;
        }
    }

    private void PrepareJump()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        Launch();
    }

    private void Launch()
    {
        state = State.JUMPING;
        rb.isKinematic = false;

        float g = Mathf.Abs(Physics.gravity.y);

        float vy = Mathf.Sqrt(2f * g * height);

        float timeUp = vy / g;
        float totalTime = timeUp * 2f;

        groundCheckOffset = totalTime / 2;
        groundCheckTimer = groundCheckOffset;

        float horizontalSpeed = distance / totalTime;

        Vector3 horizontalVelocity =
            direction.normalized * horizontalSpeed;

        Vector3 velocity =
            horizontalVelocity + Vector3.up * vy;

        rb.linearVelocity = velocity;
    }

    private void Jumping()
    {
        if (groundCheckTimer > 0)
            groundCheckTimer -= Time.deltaTime;
        else
            if (groundCheck.IsGrounded()) EndAttack();
    }

    private void EndAttack()
    {
        isAttacking = false;
        state = State.NOTHING;
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.JUMPING) return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        IDamageable damageable =
            collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}
