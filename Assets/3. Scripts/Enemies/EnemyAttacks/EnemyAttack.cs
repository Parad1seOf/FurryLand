using UnityEngine;

public abstract class EnemyAttack :MonoBehaviour
{
    public float damage;
     public bool isAttacking;

    public abstract void Attack(Vector3 target);

    public virtual void EndAttack()
    {
        isAttacking = false;
    }
}
