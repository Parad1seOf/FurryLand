// Pon este script en el trigger collider junto a la acción (OpenDoorAction, ShowMessageAction…).
// Encuentra la acción automáticamente — no hace falta arrastrar nada.
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("HUD")]
    public string interactLabel = "Interactuar";

    private InteractableAction action;

    private void Awake()
    {
        action = GetComponent<InteractableAction>();
        if (action == null)
            Debug.LogWarning($"[Interactable] '{gameObject.name}' no tiene ninguna acción en el mismo GameObject.");
    }

    public string InteractLabel => interactLabel;

    public void Interact(PlayerController player)
    {
        action?.Execute(player);
    }
}