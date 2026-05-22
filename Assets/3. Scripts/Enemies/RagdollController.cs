//using UnityEngine;
//using UnityEngine.AI;

//public class RagdollController : MonoBehaviour
//{
//    [SerializeField] private Animator animator;
//    [SerializeField] private NavMeshAgent agent;
//    [SerializeField] private Collider mainCollider;
//    [SerializeField] private MonoBehaviour[] scriptsToDisable;

//    private Rigidbody[] rigidbodies;
//    private Collider[] colliders;

//    private void Awake()
//    {
//        if (animator == null)
//            animator = GetComponentInChildren<Animator>();

//        if (agent == null)
//            agent = GetComponent<NavMeshAgent>();

//        rigidbodies = GetComponentsInChildren<Rigidbody>();
//        colliders = GetComponentsInChildren<Collider>();

//        SetRagdoll(false);
//    }

//    public void EnableRagdoll(Vector3 forceDirection, float force)
//    {
//        if (animator != null)
//            animator.enabled = false;

//        if (agent != null)
//            agent.enabled = false;

//        if (mainCollider != null)
//            mainCollider.enabled = false;

//        foreach (MonoBehaviour script in scriptsToDisable)
//        {
//            if (script != null)
//                script.enabled = false;
//        }

//        SetRagdoll(true);

//        Rigidbody hip = FindRigidbodyByName("Hip");

//        if (hip != null)
//        {
//            hip.AddForce(forceDirection.normalized * force, ForceMode.Impulse);
//        }
//    }

//    private void SetRagdoll(bool active)
//    {
//        foreach (Rigidbody rb in rigidbodies)
//        {
//            rb.isKinematic = !active;
//        }

//        foreach (Collider col in colliders)
//        {
//            if (col == mainCollider)
//                continue;

//            col.enabled = active;
//        }
//    }

//    private Rigidbody FindRigidbodyByName(string boneName)
//    {
//        foreach (Rigidbody rb in rigidbodies)
//        {
//            if (rb.name == boneName)
//                return rb;
//        }

//        return null;
//    }
//}