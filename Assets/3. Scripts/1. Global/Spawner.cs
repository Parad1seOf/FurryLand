using UnityEngine;

public class Spawner : InteractableAction
{
    [SerializeField] private float timeToSpawn = 5f;
    [SerializeField] private Transform spawnpoint;
    private float spawnTimer;
    private bool wantsToSpawn = false;
    private bool isBlocked = false;
    [SerializeField] GameObject blockingObject;
    [SerializeField] Interactable activator;
    private bool alarmed;
    private bool alreadyBroken = false;

    [SerializeField] private GameObject elephant;

    private EnemyPool pool;

    public void Start()
    {
        SpawnerManager.instance.AddSpawner(this);
        pool = EnemyPool.instance;
        AlertSystem.Instance.OnAlertTriggered += Alarmed;
        spawnTimer = timeToSpawn;
    }

    public void Update()
    {
        if (!alarmed || isBlocked) return;

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

    [ContextMenu("Block")]
    public void Block()
    {
        isBlocked = true;
        SpawnerManager.instance.AddBlockedDoor(this);
        blockingObject.SetActive(true);
        activator.enabled = false;

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Block_Door");
    }

    public void BreakBlock()
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

    public bool HasElephant()
    {
        return elephant != null;
    }

    public GameObject SpawnElephant()
    {
        //if (!alarmed) return null;
        if (isBlocked) return null;

        elephant.SetActive(true);
        elephant.GetComponent<EnemyStateMachine>().Respawn(spawnpoint.position);

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Elephant_Spawns");

        return elephant;
    }
}
