using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElephantManager : MonoBehaviour
{
    [SerializeField] private List<Spawner> spawners;
    public bool startSpawningElephants;
    private HealthSystem currentElephant;

    public static ElephantManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Update()
    {
        if (!startSpawningElephants) return;
        if (currentElephant != null) return;

        Spawner spawner = spawners.ElementAt(Random.Range(0, spawners.Count));
        currentElephant = spawner.SpawnElephant();
        if (currentElephant == null) return;
        currentElephant.OnDeath += ElephantDies;
    }

    public void ElephantDies()
    {
        currentElephant.OnDeath -= ElephantDies;
        currentElephant = null;
    }

    
}
