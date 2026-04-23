using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    [SerializeField] private int amount;

    public void OnTriggerEnter(Collider other)
    {
        PlayerWeapon weapon = other.GetComponent<PlayerWeapon>();
        if (weapon == null) return;

        weapon.GrabMagazine(amount);
    }
}
