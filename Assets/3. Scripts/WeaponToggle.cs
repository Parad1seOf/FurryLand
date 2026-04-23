using UnityEngine;

public class WeaponToggle : MonoBehaviour
{
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private float suspiciousness;

    private InputReader input;

    public bool IsWeaponDrawn => weaponObject != null && weaponObject.activeSelf;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        weaponObject?.SetActive(false); // empieza guardada
    }

    private void Update()
    {
        if (input.WeaponPressed)
            Toggle();
    }

    private void Toggle()
    {
        weaponObject?.SetActive(!weaponObject.activeSelf);
        SuspicionComponent sus = GetComponent<SuspicionComponent>();
        if (weaponObject.activeSelf) sus?.RiseSuspicion(suspiciousness);
        else sus?.LowerSuspicion(suspiciousness);
    }
}