// Acción de puerta genérica. Sirve para cualquier puerta:
// arrastra el GameObject de la puerta al campo "door" en el Inspector.
using System.Collections;
using UnityEngine;

public class OpenDoorAction : InteractableAction
{
    [Header("Puerta")]
    [Tooltip("Arrastra aquí el GameObject de la puerta (el que tiene el mesh/colisión real)")]
    public Transform door;

    [Header("Configuración")]
    public float openAngle = 90f;
    public float animSpeed = 4f;

    private bool      isOpen;
    private Quaternion closedRot;
    private Quaternion openRot;
    private Coroutine  anim;

    private void Awake()
    {
        if (door == null) return;
        closedRot = door.rotation;
        openRot   = Quaternion.Euler(door.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    public override void Execute(PlayerController player)
    {
        if (door == null)
        {
            return;
        }

        isOpen = !isOpen;
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(AnimateDoor(isOpen ? openRot : closedRot));
    }

    private IEnumerator AnimateDoor(Quaternion target)
    {
        while (Quaternion.Angle(door.rotation, target) > 0.1f)
        {
            door.rotation = Quaternion.Lerp(door.rotation, target, Time.deltaTime * animSpeed);
            yield return null;
        }
        door.rotation = target;
    }
}