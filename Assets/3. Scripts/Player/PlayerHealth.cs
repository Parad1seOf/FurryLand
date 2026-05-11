using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [SerializeField] private float timeToStartHealing = 3;
    private float timer;
    [SerializeField] private float healingPerSecond = 10;

    

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
    } 
}
