// Interfaz común de interacción. Igual que IDamageable pero para objetos del mundo.
// Cualquier objeto interactuable (puerta, palanca, pickup…) debe implementarla.
public interface IInteractable
{
    string InteractLabel { get; }          // Texto que verá el jugador en el HUD
    void   Interact(PlayerController player);
    bool CanInteract(PlayerController player);
}