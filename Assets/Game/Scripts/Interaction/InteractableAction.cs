// Clase base para todas las acciones de interacción.
// Crea un script nuevo que herede de esta clase para definir cada acción.
// Ejemplo: ShowMessageAction, OpenDoorAction, PickupAction…
using UnityEngine;

public abstract class InteractableAction : MonoBehaviour
{
    public abstract void Execute(PlayerController player);
}