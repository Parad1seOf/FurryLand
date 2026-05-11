using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AIPath : MonoBehaviour
{
    [SerializeField] private bool isLoop;
    [SerializeField] private List<Transform> points;

    public int GetClosest(Transform location)
    {
        int closestIndex = -1;
        float closestDistanceSqr = Mathf.Infinity;

        int currentIndex = 0;

        foreach (Transform t in points)
        {
            if (t == null)
            {
                currentIndex++;
                continue;
            }

            float distanceSqr = (t.position - location.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestIndex = currentIndex;
            }

            currentIndex++;
        }

        return closestIndex;
    }

    public int NextPoint(int index)
    {
        index++;
        if ((index >= points.Count) && isLoop) return 0;
        return index;
    }

    public Vector3 GetPointByIndex(int index)
    {
        return points.ElementAt(index).position;
    }
}
