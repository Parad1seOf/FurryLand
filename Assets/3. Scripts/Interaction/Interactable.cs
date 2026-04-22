// Pon este script en el trigger collider junto a la acción (OpenDoorAction, ShowMessageAction…).
// Coge la acción automáticamente del mismo GameObject — no hace falta arrastrar nada.
using System;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("HUD")]
    public string interactLabel = "Interactuar";

    [Header("Interacción")]
    public bool  holdToInteract = false;
    public float holdDuration   = 1.5f;
    public float suspiciousness;

    public string InteractLabel  => interactLabel;
    public bool   HoldToInteract => holdToInteract;
    public float  HoldDuration   => holdDuration;

    [SerializeField] private InteractableAction action;

    private void Awake()
    {
        if(action == null)
            action = GetComponent<InteractableAction>();
        if (action == null)
            Debug.LogWarning($"[Interactable] '{gameObject.name}' no tiene ninguna InteractableAction en el mismo GameObject.");
    }

    public void Interact(PlayerController player)
    {
        action?.Execute(player);
    }
}