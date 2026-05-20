using UnityEngine;

public class Spawner : InteractableAction
{
    [SerializeField] private float timeToSpawn = 5f;
    [SerializeField] private Transform spawnpoint;
    private float spawnTimer;
    private bool wantsToSpawn = false;
    private bool isBlocked = false;
    [SerializeField] private float timeToBreakBlock;
    private float breakBlockTimer;
    [SerializeField] GameObject blockingObject;
    [SerializeField] Interactable activator;
    private bool alarmed;
    private bool alreadyBroken = false;

    [SerializeField] private GameObject elephant;

    private EnemyPool pool;

    public void Start()
    {
        pool = EnemyPool.instance;
        AlertSystem.Instance.OnAlertTriggered += Alarmed;
        spawnTimer = timeToSpawn;
    }

    public void Update()
    {
        if (!alarmed) return;

        if (isBlocked)
        {
            breakBlockTimer -= Time.deltaTime;
            if (breakBlockTimer < 0) BreakBlock();
            return;
        }

        if (!wantsToSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0) wantsToSpawn = true;
            return;
        }


        GameObject enemy = pool.GetEnemy();
        if (enemy == null) return;

        enemy.GetComponent<EnemyStateMachine>().Respawn(spawnpoint.position);
        wantsToSpawn = false;
        spawnTimer = timeToSpawn;
    }

    public void Block()
    {
        isBlocked = true;
        breakBlockTimer = timeToBreakBlock;
        blockingObject.SetActive(true);
        activator.enabled = false;
    }

    private void BreakBlock()
    {
        isBlocked = false;
        blockingObject.SetActive(false);
        alreadyBroken = true;
    }

    public override void Execute(PlayerController player)
    {
        if (alreadyBroken) return;
        Block();
    }

    public void Alarmed()
    {
        alarmed = true;
    }

    public GameObject SpawnElephant()
    {
        //if (!alarmed) return null;
        if (isBlocked) return null;

        elephant.SetActive(true);
        elephant.GetComponent<EnemyStateMachine>().Respawn(spawnpoint.position);
        return elephant;
    }
}
