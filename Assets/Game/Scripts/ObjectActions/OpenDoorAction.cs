using UnityEngine;

public class OpenDoorAction : InteractableAction
{
    private bool isOpen = false;

    public override void Execute(PlayerController player)
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Puerta abierta" : "Puerta cerrada");
    }
}