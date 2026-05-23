using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    /*private ScoreManager() { }

    private static ScoreManager instance;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ScoreManager();
            return instance;
        }
        private set
        {
            instance = value;
        }
    }*/

    public static ScoreManager instance { get; private set; }

    private int totalShots = 0;
    private int hitShots = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private int killCount;

    private bool hasFurryConstitution;

    public void scoreKill()
    {
        killCount++;
    }

    public int GetKillCount()
    {
        return killCount;
    }

    public void ObtainFurryConstitution()
    {
        hasFurryConstitution = true;
    }

    public bool HasFurryConstitution()
    {
        return hasFurryConstitution;
    }

    public void RegisterShot()
    {
        totalShots++;
    }

    public void RegisterHit()
    {
        if (hitShots < totalShots)
            hitShots++;
    }

    public int GetAccuracy()
    {
        if (totalShots == 0) return 0;

        float ratio = (float) hitShots / totalShots;
        if (ratio > 1f) ratio = 1f;

        return Mathf.RoundToInt(ratio * 100f);
    }
}
