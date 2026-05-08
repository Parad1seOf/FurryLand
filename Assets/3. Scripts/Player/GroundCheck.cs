using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private LayerMask layers;
    private bool isGrounded = false;


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(point.position, radius, layers);
    }

    public bool IsGrounded() {  return isGrounded; }
}
