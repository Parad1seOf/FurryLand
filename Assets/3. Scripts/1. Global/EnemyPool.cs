using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private LinkedList<GameObject> deadRabbits;
    [SerializeField] private List<int> maxLivingRabbits = new() { 30, 40, 50 };
    private int stage = 0;
    private LinkedList<GameObject> currentLivingRabbits;
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
        currentLivingRabbits = new LinkedList<GameObject>();
        deadRabbits = new LinkedList<GameObject>();
    }

    public void AddDeadRabbit(GameObject enemy)
    {
        if (currentLivingRabbits.Contains(enemy)) currentLivingRabbits.Remove(enemy);
        deadRabbits.AddLast(enemy);
    }

    public GameObject GetEnemy()
    {
        if (currentLivingRabbits.Count > maxLivingRabbits[stage]) return null;

        if (deadRabbits.Count == 0) 
            return NewEnemy();

        GameObject enemy;
        enemy = deadRabbits.First.Value;
        deadRabbits.RemoveFirst();

        return enemy;
    }

    private GameObject NewEnemy()
    {
        return Instantiate(rabbit, transform.position, transform.rotation);
    }

    public void AddLivingRabbit(GameObject rabbit)
    {
        currentLivingRabbits.AddLast(rabbit);
    }

    public void Explosion()
    {
        if (currentLivingRabbits.Count <= stage) return;
        stage++;
    }
}
