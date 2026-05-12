using UnityEngine;

public class EnemyGunAttack : EnemyAttack
{
    [SerializeField] private GunSystem gun;
    [SerializeField] private Transform origin;

    public override void Attack(Vector3 targetDirection)
    {
        gun.TryShoot(origin.position, targetDirection);
    }
}
