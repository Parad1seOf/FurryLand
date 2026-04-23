using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GunSystem gun;

    public void GrabMagazine(int amount)
    {
        gun.AddMagazines(amount);
    }
}
