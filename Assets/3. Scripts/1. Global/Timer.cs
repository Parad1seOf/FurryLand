using UnityEngine;

public class Timer : MonoBehaviour
{
    /*private static Timer instance;

    public static Timer Instance
    {
        get
        {
            if (instance == null)
                instance = new Timer();
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private Timer() { }*/

    public static Timer instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private float time = 0;
    private bool isRunning = false;

    public void Start()
    {
        StartTimer();
    }

    public void Update()
    {
        if (isRunning) time += Time.deltaTime;
    }

    public void StartTimer()
    {
        time = 0;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public int GetMinutes()
    {
        return Mathf.FloorToInt(time / 60f);
    }

    public int GetSeconds()
    {
        return Mathf.FloorToInt(time % 60f);
    }

    public float GetTimeInSeconds()
    {
        return time;
    }
}
