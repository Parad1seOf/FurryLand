using UnityEngine;

public class SlothAttack : EnemyAttack
{
    [SerializeField] private GunSystem gun;
    [SerializeField] private Transform origin;
    [SerializeField] private float distance = 50f;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float timeToShoot = 2;
    private Vector3 target;
    private Vector3 follow;
    public float timer;
    

    private LineRenderer beam;

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
            beam.enabled = true;
        }

        this.target = target;
    }

    public void Update()
    {
        if (!isAttacking) return;

        follow = Vector3.MoveTowards( follow, target, followSpeed * Time.deltaTime);
        Beam();

        if (timer < 0)
        {
            gun.TryShoot(origin.position, target - origin.position);
            EndAttack();
        }
    }

    private void Beam()
    {
        Vector3 direction = follow - origin.position;

        direction.Normalize();

        beam.SetPosition(0, origin.position);

        if (Physics.Raycast(origin.position, direction, out RaycastHit hit))
        {
            
            beam.SetPosition(1, hit.point);
                

            PlayerTarget player = hit.collider.GetComponent<PlayerTarget>();
            if (player == null) timer = timeToShoot;
            else timer -= Time.deltaTime;
        }
        else beam.SetPosition(1, origin.position + direction.normalized * 100);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        beam.enabled = false;
    }
}
