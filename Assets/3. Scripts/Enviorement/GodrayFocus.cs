using UnityEngine;
using System.Collections;

public class GodrayFocus : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem targetParticleSystem;
    public Transform player;

    [Header("Distance Settings")]
    public float appearRadius = 15f;
    public float fullVisibleRadius = 3f;

    [Header("Timing")]
    public float stayVisibleTime = 6f;
    public float fadeOutDuration = 2f;

    [Header("Smoothness")]
    [Tooltip("Cuanto m�s alto, m�s r�pido reaccionar� el alpha.")]
    public float alphaSmoothSpeed = 5f;

    private ParticleSystem.MainModule mainModule;

    private bool sequenceStarted = false;
    private bool finished = false;

    private float currentAlpha = 0f;
    private float targetAlpha = 0f;

    private void Start()
    {
        if (targetParticleSystem == null)
        {
            enabled = false;
            return;
        }

        mainModule = targetParticleSystem.main;

        // Empezar invisible
        SetAlpha(0f);

        // Reproducir part�culas
        if (!targetParticleSystem.isPlaying)
        {
            targetParticleSystem.Play();
        }
    }

    private void Update()
    {
        if (finished || player == null)
            return;

        // Si ya empez� la secuencia final no seguimos calculando distancia
        if (!sequenceStarted)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            // Fuera del radio
            if (distance > appearRadius)
            {
                targetAlpha = 0f;
            }
            // Dentro del radio m�ximo
            else if (distance <= fullVisibleRadius)
            {
                targetAlpha = 1f;

                StartCoroutine(FinalSequence());
            }
            else
            {
                // Interpolaci�n seg�n distancia
                targetAlpha = Mathf.InverseLerp(
                    appearRadius,
                    fullVisibleRadius,
                    distance
                );
            }
        }

        // Suavizado REAL del alpha
        currentAlpha = Mathf.Lerp(
            currentAlpha,
            targetAlpha,
            Time.deltaTime * alphaSmoothSpeed
        );

        SetAlpha(currentAlpha);
    }

    private IEnumerator FinalSequence()
    {
        sequenceStarted = true;

        // Forzar alpha m�ximo
        targetAlpha = 1f;

        yield return new WaitForSeconds(stayVisibleTime);

        float startAlpha = currentAlpha;

        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeOutDuration;

            currentAlpha = Mathf.Lerp(startAlpha, 0f, t);

            SetAlpha(currentAlpha);

            yield return null;
        }

        SetAlpha(0f);

        finished = true;

        // Desactivar completamente
        gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        Color color = mainModule.startColor.color;

        color.a = Mathf.Clamp01(alpha);

        mainModule.startColor = color;
    }

    // Visualizaci�n de radios
    private void OnDrawGizmosSelected()
    {
        // Radio aparici�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, appearRadius);

        // Radio 100%
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fullVisibleRadius);
    }
}