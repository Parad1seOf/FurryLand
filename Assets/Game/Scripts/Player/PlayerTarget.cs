using UnityEngine;

public class PlayerTarget : MonoBehaviour, ITarget
{
    public Transform GetTransform()
    {
        return transform;
    }
}
