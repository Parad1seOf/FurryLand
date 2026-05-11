using UnityEngine;

public class Explosives : InteractableAction
{
    [SerializeField] private float timeToExplode;
    [SerializeField] private int explosionCount = 3;
    private int currentExplosions;
    [SerializeField] private GameObject retrievedObject;
    [SerializeField] private GameObject objectToExplode;
    [SerializeField] private GameObject trigger;
    public float timer;


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
    }

    public void Explode()
    {
        currentExplosions++;
        ComicPanelManager.Instance.ShowPhraseByID("C4_Explode");
        gameObject.SetActive(false);
        timer = timeToExplode;

        if (currentExplosions < explosionCount)
            trigger.SetActive(true);

        if (currentExplosions < explosionCount) return;

        Instantiate(retrievedObject, transform.position, transform.rotation);
        objectToExplode.SetActive(false);
    }

    public override void Execute(PlayerController player)
    {
        gameObject.SetActive(true);
        trigger.SetActive(false);
        ComicPanelManager.Instance.ShowPhraseByID("C4_Placed");
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
}
