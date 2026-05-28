using UnityEngine;

public class SimpleBreakableProp : MonoBehaviour, IDamageable
{
    public void Break()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        Break();
    }
}
