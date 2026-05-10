using System;
using UnityEngine;

public class Phase2FXManager : MonoBehaviour
{
    [SerializeField] private GameObject phase2Volume;
    [SerializeField] private WeaponToggle weaponToggle;

    void Start()
    {
        if(phase2Volume != null)
            phase2Volume.SetActive(false);

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered += EnablePhase2FX;
    }

    void Update()
    {
        if(phase2Volume != null && weaponToggle != null)
        {
            bool active = weaponToggle.IsWeaponDrawn;
            
            if (phase2Volume.activeSelf != active)
                phase2Volume.SetActive(active);
        }
    }

    private void EnablePhase2FX()
    {
        if (phase2Volume != null)
            phase2Volume.SetActive(true);
    }

    private void OnDestroy()
    {
        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
    }

}
