using UnityEngine;

public class SimpleBreakableProp : MonoBehaviour, IDamageable
{
    [Header("FX")]
    public ParticleSystem breakParticles;
    public float particlesLifetime = 3f;

    public void Break()
    {
        if (breakParticles != null)
        {
            ParticleSystem particles = Instantiate(
                breakParticles,
                transform.position,
                transform.rotation);

            Destroy(particles.gameObject, particlesLifetime);
        }

        gameObject.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        Break();
    }
}