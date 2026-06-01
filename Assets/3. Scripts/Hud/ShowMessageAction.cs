// Acción concreta: muestra un mensaje en el HUD al interactuar.
// Pon este script en el mismo GameObject que Interactable y asígnalo en su campo "Acción".
using UnityEngine;

public class ShowMessageAction : InteractableAction
{
    [Header("Mensaje")]
    [TextArea]
    public string message = "Texto del mensaje...";
    public float  duration = 3f;

    public override void Execute(PlayerController player)
    {
        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
        if (hud == null)
        {
            return;
        }
        hud.ShowMessage(message, duration);
    }
}