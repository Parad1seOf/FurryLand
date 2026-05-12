using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class EnemyPoolMember : MonoBehaviour
{
    public void Start()
    {
        GetComponent<HealthSystem>().OnDeath += Die;
    }

    public void Die()
    {
        EnemyPool.instance.AddEnemy(gameObject);
    }
}
