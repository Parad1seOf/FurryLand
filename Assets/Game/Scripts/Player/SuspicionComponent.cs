using UnityEngine;

public class SuspicionComponent : MonoBehaviour
{
    [SerializeField]
    private float suspicionLevel = 0;

    [SerializeField]
    private float minSuspicionLevel = 0;

    [SerializeField]
    private float maxSuspicionLevel = 100;

    public void riseSuspicion(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel + amount, minSuspicionLevel, maxSuspicionLevel);
    }

    public void lowerSuspicion(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel - amount, minSuspicionLevel, maxSuspicionLevel);
    }

    public float getSuspicionLevel()
    {
        return suspicionLevel;
    }

    public bool isSuspicious()
    {
        return suspicionLevel > 0;
    }
}
