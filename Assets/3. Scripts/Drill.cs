using UnityEngine;

public class Drill : InteractableAction
{
    [SerializeField] private float timeToDrill;
    [SerializeField] private GameObject retrievedObject;
    [SerializeField] private GameObject objectToExplode;
    [SerializeField] private GameObject trigger;
    public float timer;


    public void OnEnable()
    {
        timer = timeToDrill;
        AlertSystem.Instance.TriggerAlert();
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            EndDrilling();
        }
    }

    public void EndDrilling()
    {
        Instantiate(retrievedObject, transform.position, transform.rotation);
        objectToExplode.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Execute(PlayerController player)
    {
        gameObject.SetActive(true);
        trigger.SetActive(false);
    }

    [ContextMenu("Ejecutar")]
    public void Execuuute()
    {
        Execute(null);
    }
}
