using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class EnemyPoolMember : MonoBehaviour
{
    public void Start()
    {
        GetComponent<HealthSystem>().OnDeath += Die;
        EnemyPool.instance.AddLivingRabbit(gameObject);
    }

    public void Die()
    {
        EnemyPool.instance.AddDeadRabbit(gameObject);
    }
}
