using UnityEngine;

public class Explosives : InteractableAction
{
    [SerializeField] private float timeToExplode;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private GameObject[] explosionPrefabs = new GameObject[3];
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private int explosionCount = 3;
    private int currentExplosions;

    [SerializeField] private GameObject retrievedObject;
    [SerializeField] private Transform retrievedItemPoint;
    [SerializeField] private GameObject objectToExplode;
    [SerializeField] private GameObject trigger;
    public float timer;
    public bool inProgress;    

    [SerializeField] private GameObject[] visuals;
    private int visualsIndex = 0;

    private bool hasShownComicPanel = false;

    public void Start()
    {
        foreach (GameObject gameObject in visuals)
        {
            gameObject?.SetActive(false);
        }
        visuals[0].SetActive(true);
    }

    public void OnEnable()
    {
        timer = timeToExplode;
        AlertSystem.Instance.TriggerAlert();
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Explode();
        }

        if (retrievedItemPoint == null)
            retrievedItemPoint = transform;
    }

    public void Explode()
    {
        currentExplosions++;

        if (explosionSound != null && AudioManager.Instance != null)
            AudioManager.Instance.sfxSource.PlayOneShot(explosionSound);

        int index = currentExplosions - 1;

        if (explosionPrefabs[index] != null)
        {
            GameObject nuevaExplosion = Instantiate(explosionPrefabs[index], transform.position, Quaternion.identity);

            nuevaExplosion.SetActive(true);
        }

        ElephantManager.instance.wantsToSpawnElephant = true;
        ComicPanelManager.Instance?.ShowPhraseByID("C4_Explode");
        EnemyPool.instance.Explosion();

        ChangeVisual();
        if (currentExplosions == 3 && smokePrefab != null)
        {
            GameObject smokeClone = Instantiate(smokePrefab, transform.position, Quaternion.identity);
            smokeClone.SetActive(true);
        }
        
        gameObject.SetActive(false);
        timer = timeToExplode;

        if (currentExplosions < explosionCount)
        {
            trigger.SetActive(true);

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.StartC4Reminder("C4_Checkpoint", 30f);

            return;
        }

        Instantiate(retrievedObject, retrievedItemPoint.position, retrievedItemPoint.rotation);
        objectToExplode.SetActive(false);
        inProgress = false;

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Constitucion_Spawned");
    }

    private void ChangeVisual()
    {
        visuals[visualsIndex].SetActive(false);
        visualsIndex = Mathf.Min(visualsIndex + 1, visuals.Length - 1);
        visuals[visualsIndex]?.SetActive(true);
    }

    public override void Execute(PlayerController player)
    {
        inProgress = true;
        gameObject.SetActive(true);
        trigger.SetActive(false);

        if (!hasShownComicPanel)
        {
            ComicPanelManager.Instance?.ShowPhraseByID("C4_Placed");
            hasShownComicPanel = true;
        }

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.CancelC4Reminder();
    }

    [ContextMenu("Ejecutar")]
    public void Execuuute()
    {
        Execute(null);
    }

    public float GetProgress()
    {
        float progress = 0f;

        progress = Mathf.Min((currentExplosions * timeToExplode + (timeToExplode - timer)) / (timeToExplode * explosionCount), 1f);

        return progress;
    }

    public void GetWatered(float amount)
    {
        timer += amount * Time.deltaTime;
        if (timer > timeToExplode) timer = timeToExplode;
    }
}
