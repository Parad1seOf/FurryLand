using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<HealthSystem>().OnDeath += Die;
    }

    public void Die()
    {
        //SceneManager.LoadScene(0);
    }
}
