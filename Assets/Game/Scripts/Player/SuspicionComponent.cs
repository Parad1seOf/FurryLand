using UnityEngine;

public class SuspicionComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Como de sospechoso es el jugador. No tocar manualmente")]
    private float suspicionLevel = 0;

    [SerializeField]
    [Tooltip("Nivel de sospecha minimo. Si Suspicion Level tiene este valor, el jugador no esta siendo sospechoso")]
    private float minSuspicionLevel = 0;

    [SerializeField]
    [Tooltip("Valor maximo de sospecha.")]
    private float maxSuspicionLevel = 100;

    public void RiseSuspicion(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel + amount, minSuspicionLevel, maxSuspicionLevel);
    }

    public void LowerSuspicion(float amount)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel - amount, minSuspicionLevel, maxSuspicionLevel);
    }

    public float GetSuspicionLevel()
    {
        return suspicionLevel;
    }

    public bool IsSuspicious()
    {
        return suspicionLevel > minSuspicionLevel;
    }

    public void ResetSuspicionLevel()
    {
        suspicionLevel = minSuspicionLevel;
    }
}
