using UnityEngine;

public class EnemyAwarenessComponent : MonoBehaviour
{
    private float currentAwareness;
    [SerializeField] private float maxAwareness = 100f;
    [SerializeField] private float awarenessDecayPerSecond = 100f;
    private bool isAware = false;

    public bool UpdateAwareness(float amount)
    {
        isAware = true;
        currentAwareness += amount * Time.deltaTime;
        currentAwareness = Mathf.Max(0, currentAwareness);
        HUDManager.UpdateMostAwareEnemy(this);
        if (currentAwareness >= maxAwareness) return true;
        return false;
    }

    public void BecomeUnaware()
    {
        isAware = false;
    }

    public void Update()
    {
        if (currentAwareness == 0) return;

        if (!isAware)
        {
            currentAwareness -= Time.deltaTime * awarenessDecayPerSecond;
            currentAwareness = Mathf.Min(currentAwareness, 0);
        }
    }

    public float GetAwareness() { return currentAwareness; }
}
