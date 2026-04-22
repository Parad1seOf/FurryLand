using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private LinkedList<GameObject> pool;

    public static EnemyPool instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        pool = new LinkedList<GameObject>();
    }

    public void AddEnemy(GameObject enemy)
    {
        pool.AddLast(enemy);
    }

    public GameObject GetEnemy()
    {
        if (pool.Count == 0) return null;

        GameObject enemy;
        enemy = pool.First.Value;
        pool.RemoveFirst();

        return enemy;
    }
}
