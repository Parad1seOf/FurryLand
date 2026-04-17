using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Settings")]
    public float maxStamina           = 100f;
    public float staminaDrainRate     = 20f;
    public float staminaRechargeRate  = 15f;
    public float staminaRechargeDelay = 2f;

    public float Stamina           { get; private set; }
    public bool  HasStamina        => Stamina > 0f;
    public float StaminaNormalised => maxStamina > 0f ? Stamina / maxStamina : 0f;

    private float rechargeTimer;

    private void Awake() => Stamina = maxStamina;

    // Llama desde PlayerController cada Update indicando si está corriendo
    public void Tick(bool isRunning)
    {
        if (isRunning)
        {
            Stamina = Mathf.Max(Stamina - staminaDrainRate * Time.deltaTime, 0f);
            rechargeTimer = staminaRechargeDelay;
        }
        else
        {
            if (rechargeTimer > 0f)
                rechargeTimer -= Time.deltaTime;
            else
                Stamina = Mathf.Min(Stamina + staminaRechargeRate * Time.deltaTime, maxStamina);
        }
    }

    public void Restore(float amount) => Stamina = Mathf.Min(Stamina + amount, maxStamina);
    public void SetFull()             => Stamina = maxStamina;
}