using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Referencias de Juego")]
    public PlayerController playerController;
    public GunSystem        gunSystem;

    [Header("Barra de Vida")]
    public Slider        healthSlider;
    public RectTransform healthDelayedRect;
    public float         delayBeforeDrop  = 0.6f;
    public float         delayedDropSpeed = 3f;

    [Header("Barra de Stamina")]
    public Slider staminaSlider;
    public bool   hideStaminaWhenFull = true;
    public float  staminaFadeSpeed    = 3f;

    [Header("Texto de Munición")]
    public TextMeshProUGUI ammoText;
    public Color ammoColorNormal = Color.white;
    public Color ammoColorLow    = new Color(1f, 0.4f, 0.1f);

    [Header("GameManager UI")]
    public Image fadeImage;
    public Image hitFlashImage;

    #endregion

    #region Private State

    private float       delayedFillValue;
    private float       delayTimer;
    private float       previousHealth;
    private CanvasGroup staminaGroup;
    private HealthSystem healthSystem;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterUI(fadeImage, hitFlashImage);

        if (staminaSlider != null)
        {
            staminaGroup = staminaSlider.GetComponent<CanvasGroup>();
            if (staminaGroup == null)
                staminaGroup = staminaSlider.gameObject.AddComponent<CanvasGroup>();
            if (hideStaminaWhenFull)
                staminaGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        if (playerController != null)
        {
            // Start garantiza que HealthSystem ya inicializó Health en su Awake
            healthSystem = playerController.GetComponent<HealthSystem>();
            if (healthSystem != null)
                healthSystem.OnHealthChanged += OnHealthChanged;

            delayedFillValue = playerController.HealthNormalised;
            previousHealth   = playerController.HealthNormalised;
            SetDelayedRect(delayedFillValue);
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
        if (playerController != null)
        {
            UpdateHealthBar();
            UpdateStaminaBar();
        }

        if (gunSystem != null)
            UpdateAmmoText();
    }

    #endregion

    #region Health

    // El evento dispara solo cuando cambia la salud — aquí registramos el trigger del delay
    private void OnHealthChanged(float newHealth)
    {
        float normalised = playerController.HealthNormalised;
        if (normalised < previousHealth - 0.001f)
            delayTimer = delayBeforeDrop;
        previousHealth = normalised;
    }

    private void UpdateHealthBar()
    {
        if (healthSlider == null) return;

        float realHealth   = playerController.HealthNormalised;
        healthSlider.value = realHealth;

        if (healthDelayedRect == null) return;

        if (delayedFillValue > realHealth)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
                delayedFillValue = Mathf.Lerp(delayedFillValue, realHealth,
                                              Time.deltaTime * delayedDropSpeed);
        }
        else
        {
            delayedFillValue = realHealth;
        }

        SetDelayedRect(delayedFillValue);
    }

    private void SetDelayedRect(float value)
    {
        if (healthDelayedRect == null) return;
        Vector2 aMax = healthDelayedRect.anchorMax;
        aMax.x = Mathf.Clamp01(value);
        healthDelayedRect.anchorMax = aMax;
        healthDelayedRect.offsetMax = new Vector2(0f, healthDelayedRect.offsetMax.y);
    }

    #endregion

    #region Stamina

    private void UpdateStaminaBar()
    {
        if (staminaSlider == null) return;

        staminaSlider.value = playerController.StaminaNormalised;

        if (hideStaminaWhenFull && staminaGroup != null)
        {
            float targetAlpha  = playerController.StaminaNormalised < 0.99f ? 1f : 0f;
            staminaGroup.alpha = Mathf.Lerp(staminaGroup.alpha, targetAlpha,
                                            Time.deltaTime * staminaFadeSpeed);
        }
    }

    #endregion

    #region Ammo

    private void UpdateAmmoText()
    {
        if (ammoText == null) return;

        int    bullets  = gunSystem.BulletsLeft;
        int    capacity = gunSystem.MagazineCapacity;
        string mags     = gunSystem.InfiniteAmmo ? "∞" : gunSystem.MagazinesLeft.ToString();

        ammoText.text  = $"{bullets}  <size=70%><color=#AAAAAA>| {mags}</color></size>";
        ammoText.color = bullets <= Mathf.CeilToInt(capacity * 0.3f)
                         ? ammoColorLow
                         : ammoColorNormal;
    }

    #endregion

    #region Public API

    public void Setup(PlayerController player, GunSystem gun)
    {
        // Desuscribirse del anterior si existía
        if (healthSystem != null)
            healthSystem.OnHealthChanged -= OnHealthChanged;

        playerController = player;
        gunSystem        = gun;

        healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem != null)
            healthSystem.OnHealthChanged += OnHealthChanged;

        delayedFillValue = player.HealthNormalised;
        previousHealth   = player.HealthNormalised;
        SetDelayedRect(delayedFillValue);
    }

    #endregion
}