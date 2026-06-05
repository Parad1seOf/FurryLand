using UnityEngine;

public class RabbitSounds : MonoBehaviour, IEnemySounds
{
    public void PlayAttack()
    {
        AudioManager.Instance.RabbitAttack(transform.position);
    }

    public void PlayDeath()
    {
        AudioManager.Instance.RabbitDeath(transform.position);
    }

    public void PlayStep()
    {
        AudioManager.Instance.RabbitStep(transform.position);
    }
}
