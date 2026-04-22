using UnityEngine;

public class Drill : InteractableAction
{
    [SerializeField] private float timeToDrill;
    [SerializeField] private GameObject drilledObject;
    public float timer;


    public void OnEnable()
    {
        timer = timeToDrill;
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
        Instantiate(drilledObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    public override void Execute(PlayerController player)
    {
        gameObject.SetActive(true);
    }

    [ContextMenu("Ejecutar")]
    public void Execuuute()
    {
        Execute(null);
    }
}
