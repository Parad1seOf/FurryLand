// Acción de puerta genérica. Sirve para cualquier puerta:
// arrastra el GameObject de la puerta al campo "door" en el Inspector.
using System.Collections;
using UnityEngine;

public class OpenDoorAction : InteractableAction
{
    public Animator animator;
    public string clipName = "Puertas";

    public Collider doorCollider;

    private void Start()
    {
        if (animator != null)
        {
            animator.Play(clipName, 0, 0f);
            animator.speed = 0f;
        }
    }

    public override void Execute(PlayerController player)
    {
        if (animator != null)
        {
            animator.speed = 1f;
            animator.Play(clipName);

            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
            else
            {
                Collider col = animator.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
}