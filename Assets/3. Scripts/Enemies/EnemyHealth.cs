// Clase base de enemigo. Implementa IDamageable para recibir impactos de GunSystem.
// Expone el evento OnDeath para que GameManager u otros sistemas reaccionen a su muerte.
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    #region Inspector Fields

    [Header("Stats")]
    public float maxHealth = 100f;

    [Header("Death")]
    public GameObject deathVFXPrefab;
    public float      deathVFXDuration = 2f;
    public float      deathDelay       = 0f;

    #endregion

    #region State

    public float Health  { get; private set; }
    public bool  IsAlive => Health > 0f;

    public event System.Action<EnemyHealth> OnDeath;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        Health = maxHealth;
    }

    #endregion

    #region IDamageable

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        Health = Mathf.Max(Health - amount, 0f);
        OnHit(amount);

        if (!IsAlive) Die();
    }

    #endregion

    #region Overridable Callbacks

    protected virtual void OnHit(float damageReceived) { }

    protected virtual void Die()
    {
        if (ScoreManager.instance != null) ScoreManager.instance.scoreKill();

        OnDeath?.Invoke(this);
        SpawnDeathVFX();
        Destroy(gameObject, deathDelay);
    }

    #endregion

    #region Utilities

    public void ResetHealth() => Health = maxHealth;

    public float HealthNormalised => maxHealth > 0f ? Health / maxHealth : 0f;

    private void SpawnDeathVFX()
    {
        if (deathVFXPrefab == null) return;
        GameObject vfx = Instantiate(deathVFXPrefab, transform.position, transform.rotation);
        Destroy(vfx, deathVFXDuration);
    }

    #endregion

    #region Editor Gizmos

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2f,
            $"HP: {Health:F0}/{maxHealth:F0}"
        );
    }
#endif

    #endregion
}