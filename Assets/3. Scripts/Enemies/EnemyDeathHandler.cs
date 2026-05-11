using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<HealthSystem>().OnDeath += Die;
    }

    public void Die()
    {
        if (ScoreManager.instance != null) ScoreManager.instance.scoreKill();
    }
}
