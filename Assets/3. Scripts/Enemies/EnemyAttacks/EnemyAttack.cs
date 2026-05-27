using UnityEngine;

public abstract class EnemyAttack :MonoBehaviour
{
    public float damage = 10;
    public bool isAttacking;

    public abstract void Attack(Vector3 target);

    public virtual void EndAttack()
    {
        isAttacking = false;
    }
}
