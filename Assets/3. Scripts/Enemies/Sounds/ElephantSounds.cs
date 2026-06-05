using UnityEngine;

public class ElephantSounds : MonoBehaviour, IEnemySounds
{


    public void PlayAttack()
    {
        throw new System.NotImplementedException();
    }

    public void PlayDeath()
    {
        AudioManager.Instance.BigRabbitDeath(transform.position);
    }

    public void PlayStep()
    {
        AudioManager.Instance.BigRabbitStep(transform.position);
    }
}
