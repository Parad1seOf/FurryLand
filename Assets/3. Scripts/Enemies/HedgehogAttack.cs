using UnityEngine;

public class HedgehogAttack : MonoBehaviour, IAttack
{
    [SerializeField] private float damage;
    [SerializeField] private float distance;

    public void Attack(Vector3 targetDirection)
    {
        RaycastHit hit;

        // Dirección (puedes cambiarla por la que necesites)
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            Debug.Log("Impacto con: " + hit.collider.name);

            // Intentar obtener el componente HealthSystem
            HealthSystem health = hit.collider.GetComponent<HealthSystem>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
