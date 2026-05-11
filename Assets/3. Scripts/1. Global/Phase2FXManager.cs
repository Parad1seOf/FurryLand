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
            healthSystem.OnHealthChanged += UpdateHealthFX;
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
    }

    private void EnablePhase2FX()
    {
        phase2Active = true;
    }

    private void UpdateHealthFX(float amount)
    {
        if (healthVolume != null)
        {
            healthVolume.weight = 1 - healthSystem.HealthNormalised;
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= UpdateHealthFX;

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
    }
}
