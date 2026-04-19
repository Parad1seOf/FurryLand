using UnityEngine;
using System;

public class SuspicionComponent : MonoBehaviour
{
    [SerializeField] private float suspicionLevel    = 0f;
    [SerializeField] private float minSuspicionLevel = 0f;
    [SerializeField] private float maxSuspicionLevel = 100f;

    public event Action OnMaxSuspicion;

    public float SuspicionNormalised => maxSuspicionLevel > 0f
        ? suspicionLevel / maxSuspicionLevel : 0f;

    public void RiseSuspicion(float amount)
    {
        float prev = suspicionLevel;
        suspicionLevel = Mathf.Clamp(suspicionLevel + amount,
                                     minSuspicionLevel, maxSuspicionLevel);

        if (prev < maxSuspicionLevel && suspicionLevel >= maxSuspicionLevel)
            OnMaxSuspicion?.Invoke();
    }

    public void LowerSuspicion(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel - amount,
                                     minSuspicionLevel, maxSuspicionLevel);
    }

    public float GetSuspicionLevel()   => suspicionLevel;
    public bool  IsSuspicious()        => suspicionLevel > minSuspicionLevel;
    public bool  IsMaxSuspicion()      => suspicionLevel >= maxSuspicionLevel;
    public void  ResetSuspicionLevel() => suspicionLevel = minSuspicionLevel;
}