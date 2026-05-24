using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElephantManager : MonoBehaviour
{
    public bool wantsToSpawnElephant;
    [SerializeField] private float timeToSpawnMin = 1f, timeToSpawnMax = 7f;
    public float timer;
    private GameObject currentElephant;

    public static ElephantManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        ResetTimer();
    }

    public void Update()
    {
        if (!wantsToSpawnElephant) return;

        if (Timer())
            SpawnElephant();
    }

    private void SpawnElephant()
    {
        if (currentElephant == null)
        {
            Spawner spawner = SpawnerManager.instance.GetRandomElephantSpawner();
            currentElephant = spawner.SpawnElephant();
            return;
        }

        currentElephant.GetComponent<HealthSystem>().OnDeath += ElephantDies;
        wantsToSpawnElephant = false;
        ResetTimer();
    }


    public void ElephantDies()
    {
        currentElephant.GetComponent<HealthSystem>().OnDeath -= ElephantDies;
        currentElephant = null;
        wantsToSpawnElephant = true;
        timer = Random.Range(timeToSpawnMin, timeToSpawnMax);
    }

    private bool Timer()
    {
        timer -= Time.deltaTime;
        return (timer < 0);
    }

    private void ResetTimer()
    {
        timer = Random.Range(timeToSpawnMin, timeToSpawnMax);
    }
}
