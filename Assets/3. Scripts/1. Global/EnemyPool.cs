using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private LinkedList<GameObject> pool;
    [SerializeField] private GameObject rabbit;

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
        if (pool.Count == 0) 
            return NewEnemy();

        GameObject enemy;
        enemy = pool.First.Value;
        pool.RemoveFirst();

        return enemy;
    }

    private GameObject NewEnemy()
    {
        return Instantiate(rabbit, transform.position, transform.rotation);
    }
}
