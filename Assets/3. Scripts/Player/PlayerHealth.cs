using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [SerializeField] private float timeToStartHealing = 3;
    private float timer;
    [SerializeField] private float healingPerSecond = 10;

    private bool lowHealthWarningTriggered = false;
    private string lastAttackerType = "";


    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (base.IsFullHealth()) return;
        base.Restore(healingPerSecond * Time.deltaTime);
    }

    public void TakeDamage(float amount, string attackerType)
    {
        if (base.Health <= 0) return;
        lastAttackerType = attackerType;
        TakeDamage(amount);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        timer = timeToStartHealing;

        if (base.Health <= 0)
        {
            GameResultUI resultUI = FindFirstObjectByType<GameResultUI>();
            if (resultUI != null)
                resultUI.ShowResults(false, lastAttackerType);

            return;
        }

        if (base.HealthNormalised <= 0.25f && !lowHealthWarningTriggered && base.Health > 0)
        {
            lowHealthWarningTriggered = true;

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.ShowPhraseByID("Low_Health");
        }
    } 
}
