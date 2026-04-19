using UnityEngine;

public class ForbiddenArea : MonoBehaviour
{
    [SerializeField] private float suspicionValue = 50f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (AlertSystem.Instance == null) return;
        foreach (var sus in AlertSystem.Instance.GetAllSuspicions())
            sus.RiseSuspicion(suspicionValue);
    }
}