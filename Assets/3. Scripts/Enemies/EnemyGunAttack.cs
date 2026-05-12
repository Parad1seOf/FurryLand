using UnityEngine;

public class EnemyGunAttack : MonoBehaviour, IAttack
{
    [SerializeField] private GunSystem gun;
    [SerializeField] private Transform origin;

    public void Attack(Vector3 targetDirection)
    {
        gun.TryShoot(origin.position, targetDirection);
    }
}
