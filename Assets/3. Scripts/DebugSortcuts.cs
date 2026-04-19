//Shortcuts and debug...
using UnityEngine;

public class DebugShortcuts : MonoBehaviour
{
    public PlayerController playerController;
    public float healthStep = 10f;

    private void Update()
    {
        if (playerController == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) playerController.TakeDamage(healthStep);
        if (Input.GetKeyDown(KeyCode.Alpha2)) playerController.RestoreHealth(healthStep);
    }

    private void OnGUI()
    {
        if (playerController == null) return;
        GUI.Label(new Rect(10, 10, 300, 50),
            $"HP: {playerController.Health:F0} / {playerController.maxHealth:F0} (press '1' or '2')\n" +
            $"Stamina: {playerController.Stamina:F0} / {playerController.maxStamina:F0}"
        );
    }
}