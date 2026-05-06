using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BloodDecalSpawnerV2 : MonoBehaviour
{
    public GameObject decalPrefab;

    public float randomRotation = 360f;
    public float normalOffset = 0.01f;
    public float decalSize = 0.5f;

    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

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
        }
    }
}