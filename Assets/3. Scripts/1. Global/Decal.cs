using UnityEngine;

public class BloodDecalSpawner : MonoBehaviour
{
    [Header("Config")]
    public GameObject bloodDecalPrefab;
    public float maxDistance = 100f;
    public LayerMask hitLayers;

    [Header("Ajustes")]
    public float normalOffset = 0.01f;   // evita z-fighting
    public bool randomRotation = true;   // rota sobre la normal

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootDecal();
        }
    }

    void ShootDecal()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, hitLayers))
        {
            SpawnDecal(hit);
        }
    }

    void SpawnDecal(RaycastHit hit)
    {
        // Posición ligeramente separada de la superficie
        Vector3 position = hit.point + hit.normal * normalOffset;

        // ?? ROTACIÓN BASE (ajusta según tu mesh)
        // OPCIÓN 1 (la más común si tu mesh usa Y como "up"):
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        // OPCIÓN 2 (si tu mesh apunta en Z en vez de Y):
        // Quaternion rotation = Quaternion.LookRotation(hit.normal);

        // Rotación aleatoria sobre la normal (para variedad)
        if (randomRotation)
        {
            float randomAngle = Random.Range(0f, 360f);
            rotation *= Quaternion.AngleAxis(randomAngle, hit.normal);
        }

        // Instanciar
        GameObject decal = Instantiate(bloodDecalPrefab, position, rotation);

        // Opcional: parentarlo al objeto golpeado (útil si se mueve)
        decal.transform.SetParent(hit.collider.transform);
    }
}