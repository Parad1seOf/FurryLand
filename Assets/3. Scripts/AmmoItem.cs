using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    [SerializeField] private int amount;
    private static bool hasShownComicPanel = false;

    public void OnTriggerEnter(Collider other)
    {
        PlayerWeapon weapon = other.GetComponent<PlayerWeapon>();
        if (weapon == null) return;

        weapon.GrabMagazine(amount);

        if (!hasShownComicPanel && ComicPanelManager.Instance != null)
        {
            ComicPanelManager.Instance.ShowPhraseByID("Ammo_Pickup");
            hasShownComicPanel = true;
        }

        Destroy(gameObject);
    }

    public static void ResetAmmoTrigger() => hasShownComicPanel = false;
}
