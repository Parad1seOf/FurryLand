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
    public bool inProgress;    

    [SerializeField] private GameObject brazo;
    [SerializeField] private GameObject oreja;
    [SerializeField] private GameObject ortaoreja;
    [SerializeField] private GameObject cabeza;
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject pierna;
    [SerializeField] private GameObject otro_brazo;

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

        ElephantManager.instance.wantsToSpawnElephant = true;
        ComicPanelManager.Instance.ShowPhraseByID("C4_Explode");

        if (currentExplosions == 1)
        {
            brazo.SetActive(false);
            oreja.SetActive(false);
        }
        else if (currentExplosions == 2)
        {
            cabeza.SetActive(false);
            otro_brazo.SetActive(false);
            ortaoreja.SetActive(false);
        }
        else if (currentExplosions == 3)
        {
            torso.SetActive(false);
            pierna.SetActive(false);
        }

        gameObject.SetActive(false);
        timer = timeToExplode;

        if (currentExplosions < explosionCount)
        {
            trigger.SetActive(true);
            return;
        }

        Instantiate(retrievedObject, transform.position, transform.rotation);
        objectToExplode.SetActive(false);
        inProgress = false;
    }

    public override void Execute(PlayerController player)
    {
        inProgress = true;
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

    public void GetWatered(float amount)
    {
        timer += amount * Time.deltaTime;
        if (timer > timeToExplode) timer = timeToExplode;
    }
}
