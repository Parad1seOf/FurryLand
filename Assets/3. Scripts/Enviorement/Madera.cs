using UnityEngine;

public class Madera : InteractableAction
{
    private static bool firstTimePicking = true;

    public override void Execute(PlayerController player)
    {
        if (firstTimePicking)
        {
            ComicPanelManager.Instance.ShowPhraseByID("Wood_Pickup");
            firstTimePicking = false;
        }

        Destroy(gameObject);
    }
}
