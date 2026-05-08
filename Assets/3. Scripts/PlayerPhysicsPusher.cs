using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPhysicsPusher : MonoBehaviour
{
    [Header("Empuje")]
    [SerializeField] private float pushForce = 2f;
    [SerializeField] private LayerMask pushLayers = ~0;
    [SerializeField] private bool horizontalOnly = true;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;
        if ((pushLayers.value & (1 << hit.gameObject.layer)) == 0) return;

        Vector3 pushDir = hit.moveDirection;
        if (horizontalOnly) pushDir.y = 0f;

        body.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}