using UnityEditor;
using UnityEngine;

public class DropComponent : MonoBehaviour
{
    [SerializeField] private GameObject drop;
    [SerializeField][Range(0f, 100f)] private int probability;

    public void Start()
    {
        HealthSystem health = GetComponent<HealthSystem>();
        if (health == null) return;

        health.OnDeath += Drop;
    }

    public void Drop()
    {
        if (drop == null) return;

        if (Lucky())
            Instantiate(drop, transform.position, transform.rotation);
    }

    private bool Lucky()
    {
        return Random.Range(0, 101) <= probability;
    }
}
