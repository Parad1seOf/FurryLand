using UnityEngine;

public class ForbiddenArea : MonoBehaviour
{
    [SerializeField]
    private float suspicionValue;

    private void OnTriggerEnter(Collider other)
    {
        SuspicionComponent sus = other.GetComponent<SuspicionComponent>();
        if (sus == null) return;

        sus.RiseSuspicion(suspicionValue);
    }

    private void OnTriggerExit(Collider other)
    {
        SuspicionComponent sus = other.GetComponent<SuspicionComponent>();
        if (sus == null) return;

        sus.LowerSuspicion(suspicionValue);
    }
}
