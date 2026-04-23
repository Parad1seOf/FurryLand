using UnityEngine;

public class Madera : InteractableAction
{
    public override void Execute(PlayerController player)
    {
        Destroy(gameObject);
    }
}
