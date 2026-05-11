using UnityEngine;

public class AIPathingComponent : MonoBehaviour
{
    [SerializeField] AIPath path;
    [SerializeField] float reachDistance;
    private int index;

    public Vector3 GetClosestPoint()
    {
        index = path.GetClosest(transform);
        return path.GetPointByIndex(index);
    }

    public bool HasArrived()
    {
        return (reachDistance >= (path.GetPointByIndex(index) - transform.position).magnitude);
    }

    public Vector3 GetNextPoint()
    {
        index = path.NextPoint(index);
        return path.GetPointByIndex(index);
    }
}
