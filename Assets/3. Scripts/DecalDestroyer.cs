using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalDestroyer : MonoBehaviour
{
    [Header("Tiempo visible")]
    public float lifeTime = 14f;
    public float maxLifeTime = 40f;

    [Header("Fade")]
    public float fadeDuration = 3f;

    private DecalProjector decalProjector;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
    }

    private IEnumerator Start()
    {
        // Tiempo que permanece completamente visible
        float visibleTime = Random.Range(lifeTime, maxLifeTime);
        yield return new WaitForSeconds(visibleTime);

        // Fade de 100% a 0%
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = 1f - (elapsed / fadeDuration);

            if (decalProjector != null)
                decalProjector.fadeFactor = alpha;

            yield return null;
        }

        // Asegurarse de que termina totalmente invisible
        if (decalProjector != null)
            decalProjector.fadeFactor = 0f;

        Destroy(gameObject);
    }
}