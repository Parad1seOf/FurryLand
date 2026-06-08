using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BloodDecalSpawnerV2 : MonoBehaviour
{
    [Header("Decals")]
    [Tooltip("Lista de prefabs de decal de sangre. Cada impacto de partícula elige uno al azar de esta lista.")]
    public List<GameObject> decalPrefabs = new List<GameObject>();

    [Tooltip("Giro aleatorio (en grados) del decal alrededor del eje de proyección, para romper el patrón.")]
    public float randomRotation = 360f;

    [Tooltip("Separación del decal respecto a la superficie a lo largo de la normal, para evitar z-fighting.")]
    public float normalOffset = 0.01f;

    [Tooltip("Tamaño base de la HUELLA del decal (en metros). Se le aplica una variación aleatoria.")]
    public float decalSize = 0.5f;

    [Header("Decal overlap (parche para evitar zonas negras)")]
    [Tooltip("Distancia mínima (en metros) entre dos decals. Cuando uno nuevo aparece más cerca que esto " +
             "de uno ya existente, el viejo se destruye para evitar superposición. " +
             "Subir si siguen apareciendo zonas negras; bajar si quieres más densidad de sangre.")]
    public float minDecalSpacing = 0.15f;

    [Tooltip("Máximo total de decals de sangre vivos a la vez en TODA la escena (compartido entre " +
             "todos los spawners). Tope de seguridad para que no se descontrole por rendimiento.")]
    public int maxActiveDecals = 200;

    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

    private static readonly List<GameObject> activeDecals = new List<GameObject>();

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numEvents = ps.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numEvents; i++)
        {
            var e = collisionEvents[i];

            GameObject prefab = GetRandomDecalPrefab();
            if (prefab == null) continue;

            Vector3 pos = e.intersection;
            Vector3 normal = e.normal;

            Quaternion rot = Quaternion.LookRotation(-normal);

            rot *= Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, randomRotation));

            pos += normal * normalOffset;

            GameObject decal = Instantiate(prefab, pos, rot);

            float size = decalSize * UnityEngine.Random.Range(0.8f, 1.2f);
            decal.transform.localScale = new Vector3(size, size, 1f);

            if (e.colliderComponent != null)
                decal.transform.SetParent(e.colliderComponent.transform, true);

            RegisterDecal(decal);
        }
    }

    private GameObject GetRandomDecalPrefab()
    {
        if (decalPrefabs == null || decalPrefabs.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, decalPrefabs.Count);
        if (decalPrefabs[index] != null)
            return decalPrefabs[index];

        List<GameObject> valid = new List<GameObject>(decalPrefabs.Count);
        for (int i = 0; i < decalPrefabs.Count; i++)
        {
            if (decalPrefabs[i] != null)
                valid.Add(decalPrefabs[i]);
        }

        if (valid.Count == 0)
            return null;

        return valid[UnityEngine.Random.Range(0, valid.Count)];
    }

    private void RegisterDecal(GameObject newDecal)
    {
        if (newDecal == null) return;

        Vector3 newPos = newDecal.transform.position;
        float sqrMin = minDecalSpacing * minDecalSpacing;

        for (int i = activeDecals.Count - 1; i >= 0; i--)
        {
            GameObject existing = activeDecals[i];
            if (existing == null)
            {
                activeDecals.RemoveAt(i);
                continue;
            }

            if ((existing.transform.position - newPos).sqrMagnitude < sqrMin)
            {
                Destroy(existing);
                activeDecals.RemoveAt(i);
            }
        }

        activeDecals.Add(newDecal);

        while (activeDecals.Count > maxActiveDecals)
        {
            GameObject oldest = activeDecals[0];
            activeDecals.RemoveAt(0);
            if (oldest != null)
                Destroy(oldest);
        }
    }
}