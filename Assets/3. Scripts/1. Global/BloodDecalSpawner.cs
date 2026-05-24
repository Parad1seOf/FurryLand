using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BloodDecalSpawnerV2 : MonoBehaviour
{
    public GameObject decalPrefab;

    public float randomRotation = 360f;
    public float normalOffset = 0.01f;
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

    // Registro global compartido por todos los spawners. Lista (no Queue) para poder iterar y
    // eliminar elementos del medio cuando detectamos solape.
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

            Vector3 pos = e.intersection;
            Vector3 normal = e.normal;

            Quaternion rot = Quaternion.LookRotation(normal);
            rot *= Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, randomRotation));

            pos += normal * normalOffset;

            GameObject decal = Instantiate(decalPrefab, pos, rot);

            float size = decalSize * UnityEngine.Random.Range(0.8f, 1.2f);
            decal.transform.localScale = Vector3.one * size;

            if (e.colliderComponent != null)
                decal.transform.SetParent(e.colliderComponent.transform);

            RegisterDecal(decal);
        }
    }

    // PARCHE (Bug fix decals negros): el problema raíz es que dos URP Decal Projectors solapados
    // producen artefactos negros por culpa del blending del DBuffer. Evitamos el solape destruyendo
    // cualquier decal anterior que esté más cerca de `minDecalSpacing` del nuevo. Además, mantenemos
    // un tope total `maxActiveDecals` como red de seguridad para que no se acumulen indefinidamente.
    private void RegisterDecal(GameObject newDecal)
    {
        if (newDecal == null) return;

        Vector3 newPos = newDecal.transform.position;
        float sqrMin = minDecalSpacing * minDecalSpacing;

        // 1) Destruir cualquier decal previo demasiado cerca del nuevo (y de paso limpiar nulls)
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

        // 2) Registrar el nuevo
        activeDecals.Add(newDecal);

        // 3) Red de seguridad: si pasamos del tope global, eliminamos los más antiguos (FIFO)
        while (activeDecals.Count > maxActiveDecals)
        {
            GameObject oldest = activeDecals[0];
            activeDecals.RemoveAt(0);
            if (oldest != null)
                Destroy(oldest);
        }
    }
}