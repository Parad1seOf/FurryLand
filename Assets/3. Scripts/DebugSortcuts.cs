using UnityEngine;

public class DebugShortcuts : MonoBehaviour
{
    public PlayerController playerController;
    public float healthStep = 10f;

    private void Start()
    {
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (playerController == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) playerController.TakeDamage(healthStep);
        if (Input.GetKeyDown(KeyCode.Alpha2)) playerController.RestoreHealth(healthStep);
        if (Input.GetKeyDown(KeyCode.M)) playerController.GetInventory()?.AddItem(ItemType.Wood, 1);
    }

    //private void OnGUI()
    //{
    //    if (playerController == null) return;

    //    InventorySystem inv = playerController.GetInventory();
    //    int wood = inv != null ? inv.GetCount(ItemType.Wood) : 0;

    //    GUI.Label(new Rect(10, 10, 300, 80),
    //       // $"HP: {playerController.Health:F0} / {playerController.maxHealth:F0} (press '1' or '2')\n" +
    //        //$"Stamina: {playerController.Stamina:F0} / {playerController.maxStamina:F0}\n" +
    //        //$"Wood: {wood} (press 'M')"
    //    );
    //}
}