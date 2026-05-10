using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Phase2FXManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume phase2Volume;
    [SerializeField] private WeaponToggle weaponToggle;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Damage Configuration")]
    [SerializeField] float increaseAmount = 0.5f;
    [SerializeField] float decaySpeed = 0.5f;
    [SerializeField] float delay = 3f;

    private float currentWeight = 0f;
    private float timer = 0f;

    void Start()
    {
        if (phase2Volume != null)
        {
            phase2Volume.gameObject.SetActive(false);
            phase2Volume.weight = 0;
        }

        if (healthSystem != null)
            healthSystem.OnDamaged += OnPlayerTakeDamage;


        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered += EnablePhase2FX;
    }

    void Update()
    {
        if (phase2Volume == null) return;

       bool active = weaponToggle.IsWeaponDrawn || currentWeight > 0.01f;
            
        if (phase2Volume.gameObject.activeSelf != active)
            phase2Volume.gameObject.SetActive(active);

        if (timer < delay)
            timer += Time.deltaTime;

        else if (currentWeight > 0f)
        {
            currentWeight -= decaySpeed * Time.deltaTime;
            currentWeight = Mathf.Max(currentWeight, 0f);
        }
    }

    private void EnablePhase2FX()
    {
        OnPlayerTakeDamage();
        /*if (phase2Volume != null)
            phase2Volume.SetActive(true);*/
    }

    private void OnDestroy()
    {
        if(healthSystem != null)
            healthSystem.OnDamaged -= OnPlayerTakeDamage;

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
    }

    public void OnPlayerTakeDamage()
    {
        currentWeight = Mathf.Clamp01(currentWeight + increaseAmount);
        timer = 0f;
    }
}
