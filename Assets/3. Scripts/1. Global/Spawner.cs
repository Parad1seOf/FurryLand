using UnityEngine;

public class Spawner : InteractableAction
{
    [SerializeField] private float timeToSpawn = 5f;
    private float spawnTimer;
    private bool wantsToSpawn = false;
    private bool isBlocked = false;
    [SerializeField] private float timeToBreakBlock;
    private float breakBlockTimer;
    [SerializeField] GameObject blockingObject;
    [SerializeField] Interactable activator;
    private bool alarmed;

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

        enemy.GetComponent<EnemyStateMachine>().Respawn(transform.position);
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
    }

    public override void Execute(PlayerController player)
    {
        Block();
    }

    public void Alarmed()
    {
        alarmed = true;
    }
}
