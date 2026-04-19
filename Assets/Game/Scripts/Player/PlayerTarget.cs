using UnityEngine;

public class PlayerTarget : MonoBehaviour, ITarget
{
    [SerializeField] private Transform targetPoint;

    public void Start()
    {
        if (targetPoint == null)
            targetPoint = transform;
    }

    public Transform GetTransform()
    {
        return targetPoint;
    }
}
