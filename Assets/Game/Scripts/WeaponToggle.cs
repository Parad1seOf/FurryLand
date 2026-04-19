using UnityEngine;

public class WeaponToggle : MonoBehaviour
{
    [SerializeField] private GameObject weaponObject;

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
            weaponObject?.SetActive(!weaponObject.activeSelf);
    }
}