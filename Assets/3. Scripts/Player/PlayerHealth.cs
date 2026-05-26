using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [SerializeField] private float timeToStartHealing = 3;
    private float timer;
    [SerializeField] private float healingPerSecond = 10;

    private bool lowHealthWarningTriggered = false;



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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        timer = timeToStartHealing;

        if (base.HealthNormalised <= 0.25f && !lowHealthWarningTriggered && base.Health > 0)
        {
            lowHealthWarningTriggered = true;

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.ShowPhraseByID("Low_Health");
        }
    } 
}
