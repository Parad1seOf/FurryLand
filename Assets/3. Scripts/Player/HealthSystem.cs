using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;

    public float Health          { get; private set; }
    public bool  IsAlive         => Health > 0f;
    public float HealthNormalised => maxHealth > 0f ? Health / maxHealth : 0f;

    public event Action       OnDamaged;
    public event Action       OnDeath;
    public event Action<float> OnHealthChanged;

    private void Awake() => Health = maxHealth;

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        Health = Mathf.Max(Health - amount, 0f);
        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(Health);

        if (!IsAlive) Die();
    }

    public void Restore(float amount)
    {
        Health = Mathf.Min(Health + amount, maxHealth);
        OnHealthChanged?.Invoke(Health);
    }

    public void SetFull()
    {
        Health = maxHealth;
        OnHealthChanged?.Invoke(Health);
    }

    protected virtual void Die() => OnDeath?.Invoke();

    [ContextMenu("Kill")]
    public void Kill()
    {
        Die();
    }

    public bool IsFullHealth() { return Health == maxHealth; }
}