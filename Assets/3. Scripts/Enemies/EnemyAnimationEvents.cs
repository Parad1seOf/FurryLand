using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    public void Attack()
    {
        enemy.GetComponent<IAnimatedAttack>().AnimatedAttack();
    }

    public void PlayStepSound()
    {

    }
}
