using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 2f;
    [SerializeField] Explosives target;
    [SerializeField] ParticleSystem particles;


    public void StartWaterGun()
    {
        if (particles == null) return;
        particles.Play();
    }

    public void Water()
    {
        target.GetWatered(damagePerSecond);
        GetComponent<AIMovementComponent>().LookAt(target.transform.position);
    }

    public void StopWaterGun()
    {
        if (particles == null) return;
        particles.Stop();
    }
}
