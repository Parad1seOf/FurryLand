using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("RectTransform donde se instancian los marcadores. Debe estar dentro de la mascara circular.")]
    public RectTransform minimapArea;

    [Tooltip("Marcador del jugador (en el centro). Opcional.")]
    public RectTransform playerMarker;

    [Tooltip("Transform del jugador en el mundo.")]
    public Transform playerTransform;

    [Tooltip("Para rotar con el jugador. Asigna su FirstPersonLook.")]
    public FirstPersonLook playerLook;

    [Header("Sprite por defecto")]
    public Sprite defaultMarkerSprite;

    [Header("Configuracion")]
    [Tooltip("Radio del mundo (en unidades) que cubre el minimapa.")]
    public float worldRadius = 30f;

    [Tooltip("Radio del minimapa en pixeles UI. Igualalo al radio del RectTransform circular.")]
    public float uiRadius = 100f;

    [Tooltip("True = el mapa rota con el jugador (forward del jugador siempre arriba).")]
    public bool rotateWithPlayer = true;

    [Tooltip("True = los enemigos fuera del rango se muestran pegados al borde. False = se ocultan.")]
    public bool clampToEdge = false;

    [Tooltip("Cada cuanto sincroniza la lista de targets (segundos).")]
    public float refreshInterval = 0.2f;

    private readonly Dictionary<MinimapTarget, RectTransform> markers = new();
    private float refreshTimer;

    private void Update()
    {
        if (playerTransform == null || minimapArea == null) return;

        refreshTimer -= Time.deltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = refreshInterval;
            SyncMarkers();
        }

        UpdateMarkers();
        UpdatePlayerMarker();
    }

    private void SyncMarkers()
    {
        // Anadir nuevos
        foreach (MinimapTarget t in MinimapTarget.All)
        {
            if (t != null && !markers.ContainsKey(t))
                markers[t] = CreateMarker(t);
        }

        // Quitar los que se fueron (muertos / desactivados / null)
        List<MinimapTarget> toRemove = null;
        foreach (var kvp in markers)
        {
            if (kvp.Key == null || !kvp.Key.isActiveAndEnabled)
            {
                toRemove ??= new List<MinimapTarget>();
                toRemove.Add(kvp.Key);
            }
        }
        if (toRemove != null)
        {
            foreach (var t in toRemove)
            {
                if (markers.TryGetValue(t, out var rt) && rt != null)
                    Destroy(rt.gameObject);
                markers.Remove(t);
            }
        }
    }

    private RectTransform CreateMarker(MinimapTarget target)
    {
        GameObject go = new GameObject($"Marker_{target.name}",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(minimapArea, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(target.size, target.size);

        Image img = go.GetComponent<Image>();
        img.sprite = target.customSprite != null ? target.customSprite : defaultMarkerSprite;
        img.color  = target.color;
        img.raycastTarget = false;
        return rt;
    }

    private void UpdateMarkers()
    {
        float yawRad = (rotateWithPlayer && playerLook != null)
            ? playerLook.Yaw * Mathf.Deg2Rad : 0f;
        float cos = Mathf.Cos(yawRad);
        float sin = Mathf.Sin(yawRad);
        float scale = uiRadius / Mathf.Max(0.0001f, worldRadius);

        foreach (var kvp in markers)
        {
            MinimapTarget t = kvp.Key;
            RectTransform rt = kvp.Value;
            if (t == null || rt == null) continue;

            Vector3 delta = t.transform.position - playerTransform.position;
            float dx = delta.x;
            float dz = delta.z;

            if (rotateWithPlayer)
            {
                // Rota el mundo para que el forward del jugador sea +Y en la UI
                float rx = dx * cos - dz * sin;
                float rz = dx * sin + dz * cos;
                dx = rx; dz = rz;
            }

            Vector2 pos = new Vector2(dx, dz) * scale;

            if (pos.magnitude > uiRadius)
            {
                if (clampToEdge)
                {
                    pos = pos.normalized * uiRadius;
                    if (!rt.gameObject.activeSelf) rt.gameObject.SetActive(true);
                }
                else
                {
                    if (rt.gameObject.activeSelf) rt.gameObject.SetActive(false);
                    continue;
                }
            }
            else if (!rt.gameObject.activeSelf) rt.gameObject.SetActive(true);

            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(t.size, t.size);

            // Por si cambias el color en runtime
            Image img = rt.GetComponent<Image>();
            if (img != null && img.color != t.color) img.color = t.color;
        }
    }

    private void UpdatePlayerMarker()
    {
        if (playerMarker == null) return;

        // Si el mapa rota con el jugador, el marcador siempre apunta arriba.
        // Si no, gira segun el yaw del jugador.
        if (rotateWithPlayer || playerLook == null)
            playerMarker.localRotation = Quaternion.identity;
        else
            playerMarker.localRotation = Quaternion.Euler(0f, 0f, -playerLook.Yaw);
    }

    // Helpers publicos por si quieres zoom dinamico desde otro sitio
    public void SetZoom(float newWorldRadius) => worldRadius = Mathf.Max(1f, newWorldRadius);
    public void ToggleRotation() => rotateWithPlayer = !rotateWithPlayer;
}