using UnityEngine;

public class PerezosoEnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private EnemyStateMachine sloth;
    [SerializeField] private Transform spawnPoint;

    [Header("Detección del jugador")]
    [Tooltip("Radio alrededor del spawner. Si el jugador está dentro, NO se spawnea.")]
    [SerializeField] private float playerDetectionRadius = 8f;

    [Tooltip("Tag del jugador. Por defecto 'Player'.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Tiempo de respawn tras muerte")]
    [Tooltip("Tiempo mínimo (segundos) que espera el spawner tras la muerte del anterior antes de volver a spawnear.")]
    [SerializeField] private float minRespawnDelay = 3f;

    [Tooltip("Tiempo máximo (segundos) que espera el spawner tras la muerte del anterior antes de volver a spawnear.")]
    [SerializeField] private float maxRespawnDelay = 7f;

    [SerializeField] private float minSecondDelay = 1f;
    [SerializeField] private float maxSecondDelay = 2f;

    private Transform player;
    private bool isDead = false;
    private float respawnTimer = 0f;
    private float secondTimer = 0f;


    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;

        if (sloth != null)
            sloth.GetComponent<HealthSystem>().OnDeath += HandleSpawnedEnemyDeath;

        if (spawnPoint == null) spawnPoint = transform;

        if (!sloth.gameObject.activeSelf) AlertSystem.Instance.OnAlertTriggered += HandleSpawnedEnemyDeath;
    }

    private void Update()
    {
        if (!isDead) return;
        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;
            return;
        }
        if (IsPlayerInsideRadius())
        {
            secondTimer = Random.Range(minSecondDelay, maxSecondDelay);
            return;
        }

        if (secondTimer > 0f)
        {
            secondTimer -= Time.deltaTime;
            return;
        }

        SpawnEnemy();
    }

    private bool IsPlayerInsideRadius()
    {
        if (player == null) return false;
        float r = playerDetectionRadius;
        return (player.position - transform.position).sqrMagnitude < r * r;
    }

    private void SpawnEnemy()
    {
        sloth.gameObject.SetActive(true);
        sloth.Respawn(spawnPoint.position);
        isDead = false;
    }

    private void HandleSpawnedEnemyDeath()
    {
        respawnTimer = Random.Range(minRespawnDelay, maxRespawnDelay);
        isDead = true;
    }

    private void OnDisable()
    {
        if (sloth != null)
            sloth.GetComponent<HealthSystem>().OnDeath -= HandleSpawnedEnemyDeath;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}