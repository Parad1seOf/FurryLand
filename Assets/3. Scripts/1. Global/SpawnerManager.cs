using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    private List<Spawner> spawns;
    private List<Spawner> blockedDoors;
    private bool alerted;
    [SerializeField] private float timeToBreakBlock = 20f;
    public float timer;

    public static SpawnerManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        spawns = new List<Spawner>();
        blockedDoors = new List<Spawner>();
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AlertSystem.Instance.OnAlertTriggered += GetAlerted;
        timer = timeToBreakBlock;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alerted) return;
        if (blockedDoors.Count != 0)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                timer = timeToBreakBlock;
                BreakBlock();
            }
        }
    }

    public void AddSpawner(Spawner spawner)
    {
        spawns.Add(spawner);
    }

    public void AddBlockedDoor(Spawner spawner)
    {
        blockedDoors.Add(spawner);
    }

    public Spawner GetRandomElephantSpawner()
    {
        return null;
    }

    public void BreakBlock()
    {
        Spawner spawner = blockedDoors.ElementAt(Random.Range(0, blockedDoors.Count));
        blockedDoors.Remove(spawner);
        spawner.BreakBlock();
        return;
    }

    public void GetAlerted()
    {
        alerted = true;
    }
}
