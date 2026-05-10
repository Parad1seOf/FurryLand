using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Phase2FXManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume phase2Volume;
    [SerializeField] private Volume healthVolume;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Damage Configuration")]
    [SerializeField] private float increaseAmount = 0.2f;
    [SerializeField] private float decaySpeed = 0.25f;
    [SerializeField] private float delay = 3f;

    private float currentWeight = 0f;
    private float timer = 0f;

    private bool phase2Active = false;

    void Start()
    {
        if (phase2Volume != null)
        {
            phase2Volume.gameObject.SetActive(false);
            phase2Volume.weight = 0f;
        }

        if (healthVolume != null)
            healthVolume.weight = 0f;

        if (healthSystem != null)
        {
            healthSystem.OnDamaged += OnPlayerTakeDamage;
        }

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered += EnablePhase2FX;
    }

    void Update()
    {
        if (phase2Volume != null)
        {
            bool active = phase2Active;

            if (phase2Volume.gameObject.activeSelf != active)
                phase2Volume.gameObject.SetActive(active);

            phase2Volume.weight = active ? 1f : 0f;
        }

        timer += Time.deltaTime;

        if (healthSystem != null && timer >= delay && healthSystem.Health < healthSystem.maxHealth)
        {
            float decayAmount = decaySpeed * Time.deltaTime;

            currentWeight = Mathf.Max(currentWeight - decayAmount, 0f);

            float healthToRestore = decayAmount * healthSystem.maxHealth;
            healthSystem.Restore(healthToRestore);

            UpdateHealthFX();
        }
    }

    private void EnablePhase2FX()
    {
        phase2Active = true;
    }

    public void OnPlayerTakeDamage()
    {
        currentWeight += increaseAmount;
        currentWeight = Mathf.Clamp01(currentWeight);

        timer = 0f;

        UpdateHealthFX();
    }

    private void UpdateHealthFX()
    {
        if (healthVolume != null)
        {
            healthVolume.weight = currentWeight;
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnDamaged -= OnPlayerTakeDamage;

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
    }
}
