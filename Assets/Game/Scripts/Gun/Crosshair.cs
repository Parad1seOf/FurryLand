// Lee el estado de movimiento de PlayerController para animar el crosshair del HUD... idle andando etc
// Funciona de forma autónoma; no depende de GunSystem ni de GameManager... rectangulos ene l canvas...
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    public PlayerController playerController;

    [Header("Líneas del Crosshair")]
    public RectTransform lineTop;
    public RectTransform lineBottom;
    public RectTransform lineLeft;
    public RectTransform lineRight;

    [Header("Tamaños")]
    public float idleSize = 0f;
    public float walkSize = 30f;
    public float runSize  = 60f;

    [Header("Gap")]
    public float gap = 0f;

    [Header("Animación")]
    public float animationSpeed = 5f;

    #endregion

    #region Private State

    private float targetSize;
    private float currentSize;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        currentSize = idleSize;
        targetSize  = idleSize;
        Apply(currentSize);
    }

    private void Update()
    {
        UpdateTarget();
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * animationSpeed);
        Apply(currentSize);
    }

    #endregion

    #region Private Methods

    private void UpdateTarget()
    {
        if (playerController == null) return;

        if      (playerController.IsRunning) targetSize = runSize;
        else if (playerController.IsWalking) targetSize = walkSize;
        else                                 targetSize = idleSize;
    }

    private void Apply(float size)
    {
        float offset = size + gap;
        if (lineTop    != null) lineTop.anchoredPosition    = new Vector2( 0,       offset);
        if (lineBottom != null) lineBottom.anchoredPosition = new Vector2( 0,      -offset);
        if (lineLeft   != null) lineLeft.anchoredPosition   = new Vector2(-offset,  0);
        if (lineRight  != null) lineRight.anchoredPosition  = new Vector2( offset,  0);
    }

    #endregion

    #region Public API

    public void Kick(float kickSize)
    {
        currentSize = kickSize;
    }

    #endregion
}