using UnityEngine;

public class SlothSounds : MonoBehaviour, IEnemySounds
{
    public void PlayAttack()
    {
        AudioManager.Instance.SniperShoot(transform.position);
    }

    public void PlayDeath()
    {
        AudioManager.Instance.RabbitDeath(transform.position);
    }

    public void PlayStep()
    {
        throw new System.NotImplementedException();
    }
}
