using UnityEngine;
using System;

public class SuspiciousActionsComponent : MonoBehaviour
{
    [Header("Tasas de sospecha por segundo (visión)")]
    public float weaponDrawnRate = 50f;
    public float runningRate     = 10f;

    [Header("Golpes instantáneos por visión")]
    public float interactValue = 30f;

    [Header("Golpes instantáneos por sonido")]
    public float shootValue = 40f;

    private WeaponToggle     weapon;
    private PlayerController player;
    private InputReader      input;

    // Eventos — los enemigos se suscriben y reaccionan al instante
    public event Action<float> OnSoundAction;   // disparo, explosión...
    public event Action<float> OnVisionAction;  // interactuar...

    private void Awake()
    {
        weapon = GetComponent<WeaponToggle>();
        player = GetComponent<PlayerController>();
        input  = GetComponent<InputReader>();
    }

    private void Update()
    {
        if (input.FirePressed)     OnSoundAction?.Invoke(shootValue);
        if (input.InteractPressed) OnVisionAction?.Invoke(interactValue);
    }

    public float GetContinuousRate()
    {
        float rate = 0f;
        if (weapon != null && weapon.IsWeaponDrawn) rate += weaponDrawnRate;
        if (player != null && player.IsRunning)     rate += runningRate;
        return rate;
    }
}