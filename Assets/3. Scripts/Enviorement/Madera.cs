using UnityEngine;

public class Madera : InteractableAction
{
    private static bool firstTimePicking = true;

    public override void Execute(PlayerController player)
    {
        if (firstTimePicking)
        {
            ComicPanelManager.Instance.ShowPhrase("¡Madera! Puede ser útil...");
            firstTimePicking = false;
        }

        Destroy(gameObject);
    }
}
