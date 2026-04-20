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

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private int killCount;

    public void scoreKill()
    {
        killCount++;
    }

    public int GetKillCount()
    {
        return killCount;
    }
}
