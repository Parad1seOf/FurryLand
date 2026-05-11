using UnityEngine;

public class PerezosoEnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject enemyPrefab;

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

    private Transform player;
    private GameObject currentEnemy;
    private HealthSystem currentEnemyHealth;

    private float respawnTimer = 0f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;
    }

    private void Update()
    {
        if (currentEnemy != null) return;
        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;
            return;
        }
        if (IsPlayerInsideRadius()) return;

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
        if (enemyPrefab == null) return;

        currentEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

        currentEnemyHealth = currentEnemy.GetComponent<HealthSystem>();
        if (currentEnemyHealth != null)
            currentEnemyHealth.OnDeath += HandleSpawnedEnemyDeath;
    }

    private void HandleSpawnedEnemyDeath()
    {
        if (currentEnemyHealth != null)
            currentEnemyHealth.OnDeath -= HandleSpawnedEnemyDeath;

        currentEnemy = null;
        currentEnemyHealth = null;

        respawnTimer = Random.Range(minRespawnDelay, maxRespawnDelay);
    }

    private void OnDisable()
    {
        if (currentEnemyHealth != null)
            currentEnemyHealth.OnDeath -= HandleSpawnedEnemyDeath;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}