using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightEasterEgg : MonoBehaviour
{
    [Header("References")]
    public GameObject bush;
    public Image fadePanel;

    [Header("Night Environment")]
    public Light directionalLight;
    public Color nightDirectionalLightColor = Color.blue;
    public Material nightSkybox;

    [Header("Environment Lighting")]
    public Color nightSkyColor = Color.black;
    public Color nightEquatorColor = Color.black;
    public Color nightGroundColor = Color.black;

    [Header("Farola")]
    public Material emissiveMaterial;

    [Header("Settings")]
    public float requiredTimeInBush = 5f;
    public float fadeDuration = 1f;

    private float currentTimeInBush = 0f;
    private bool playerInside = false;
    private bool isTransitioning = false;

    private List<GameObject> lightLayerObjects = new List<GameObject>();

    private void Start()
    {
        Debug.Log("[NightEasterEgg] Script iniciado.");

        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
        }

        if (emissiveMaterial != null)
        {
            emissiveMaterial.DisableKeyword("_EMISSION");
        }

        DisableLightLayerObjects();
    }

    private void Update()
    {
        if (isTransitioning)
            return;

        if (playerInside)
        {
            currentTimeInBush += Time.deltaTime;

            if (currentTimeInBush >= requiredTimeInBush)
            {
                Debug.Log("[NightEasterEgg] Empieza el cambio.");
                StartCoroutine(NightTransition());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        Debug.Log("[NightEasterEgg] Entra en arbusto.");

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        Debug.Log("[NightEasterEgg] Sale del arbusto. Temporizador pausado.");

        playerInside = false;
    }

    private IEnumerator NightTransition()
    {
        isTransitioning = true;

        Debug.Log("[NightEasterEgg] Fade a negro.");

        yield return StartCoroutine(Fade(0f, 1f));

        Debug.Log("[NightEasterEgg] CAMBIA.");

        ApplyNightChanges();

        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log("[NightEasterEgg] Easter Egg completado.");

        gameObject.SetActive(false);
    }

    private void ApplyNightChanges()
    {
        Debug.Log("[NightEasterEgg] Ejecutando cambios nocturnos.");

        if (directionalLight != null)
        {
            directionalLight.color = nightDirectionalLightColor;
            Debug.Log("[NightEasterEgg] Color del Directional Light cambiado.");
        }

        if (nightSkybox != null)
        {
            RenderSettings.skybox = nightSkybox;
            Debug.Log("[NightEasterEgg] Skybox cambiado.");
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

        RenderSettings.ambientSkyColor = nightSkyColor;
        RenderSettings.ambientEquatorColor = nightEquatorColor;
        RenderSettings.ambientGroundColor = nightGroundColor;

        Debug.Log("[NightEasterEgg] Environment Lighting cambiado.");

        EnableLightLayerObjects();

        if (emissiveMaterial != null)
        {
            emissiveMaterial.EnableKeyword("_EMISSION");
            Debug.Log("[NightEasterEgg] Emission activada en el material de la farola.");
        }

        DynamicGI.UpdateEnvironment();
    }

    private void DisableLightLayerObjects()
    {
        int lightLayer = LayerMask.NameToLayer("Luz");

        if (lightLayer == -1)
        {
            Debug.LogWarning("[NightEasterEgg] No existe el layer 'Luz'.");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == lightLayer)
            {
                lightLayerObjects.Add(obj);
                obj.SetActive(false);
            }
        }

        Debug.Log("[NightEasterEgg] Objetos del layer 'Luz' desactivados: " + lightLayerObjects.Count);
    }

    private void EnableLightLayerObjects()
    {
        foreach (GameObject obj in lightLayerObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        Debug.Log("[NightEasterEgg] Objetos del layer 'Luz' activados: " + lightLayerObjects.Count);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadePanel == null)
            yield break;

        float timer = 0f;
        Color color = fadePanel.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                endAlpha,
                timer / fadeDuration
            );

            fadePanel.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadePanel.color = color;
    }
}