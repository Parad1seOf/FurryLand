// Lee el estado de movimiento de PlayerController para animar el crosshair del HUD... idle andando etc
// Funciona de forma autonoma; no depende de GunSystem ni de GameManager... 4 esquinas en el canvas.
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    #region Inspector Fields

    [Header("References")]
    public PlayerController playerController;

    [Header("Esquinas del Crosshair")]
    public RectTransform cornerTopLeft;
    public RectTransform cornerTopRight;
    public RectTransform cornerBottomLeft;
    public RectTransform cornerBottomRight;

    [Header("Tamanos")]
    public float idleSize = 0f;
    public float walkSize = 30f;
    public float runSize  = 60f;

    [Header("Gap")]
    public float gap = 0f;

    [Header("Animacion")]
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
        if (cornerTopLeft     != null) cornerTopLeft.anchoredPosition     = new Vector2(-offset,  offset);
        if (cornerTopRight    != null) cornerTopRight.anchoredPosition    = new Vector2( offset,  offset);
        if (cornerBottomLeft  != null) cornerBottomLeft.anchoredPosition  = new Vector2(-offset, -offset);
        if (cornerBottomRight != null) cornerBottomRight.anchoredPosition = new Vector2( offset, -offset);
    }

    #endregion

    #region Public API

    public void Kick(float kickSize)
    {
        currentSize = kickSize;
    }

    #endregion
}