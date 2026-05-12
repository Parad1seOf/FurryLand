using UnityEngine;

public abstract class EnemyAttack :MonoBehaviour
{
    public float damage;
    [HideInInspector] public bool isAttacking;

    public abstract void Attack(Vector3 targetDirection);
}
