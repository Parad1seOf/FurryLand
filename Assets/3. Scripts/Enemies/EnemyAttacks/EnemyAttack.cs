using UnityEngine;

public abstract class EnemyAttack :MonoBehaviour
{
    public bool isAttacking;

    public abstract void Attack(Vector3 target);

    public virtual void EndAttack()
    {
        isAttacking = false;
    }
}
