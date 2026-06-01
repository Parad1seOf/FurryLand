using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Interact Label")]
    public TextMeshProUGUI interactLabel;

    [Header("Hold Progress")]
    [Tooltip("Image tipo Filled · Fill Method: Radial 360 · Fill Origin: Top")]
    public Image holdProgressRing;          // asignar en Inspector

    [Header("Weapon Hold Progress")]
    [Tooltip("Image tipo Filled · Fill Method: Radial 360 · Fill Origin: Top — radial específico para sacar el arma")]
    public Image weaponHoldProgressRing;    // asignar en Inspector

    [Header("Message")]
    public TextMeshProUGUI messageText;

    [Header("GameManager UI")]
    public Image fadeImage;
    public Image hitFlashImage;

    [Header("Enemy suspicion")]
    [SerializeField] private TextMeshProUGUI enemySuspicion;
    [SerializeField] private Image questionMarkEmpty;
    [SerializeField] private Image questionMarkFill;
    [SerializeField] private Image exclamationMark;
    [SerializeField] private float exclamationMarkTime = 2f;

    [Header("ExplosivesProgression")]
    [SerializeField] private GameObject explosivesProgressionSliderGameObject;
    [SerializeField] private Slider explosivesProgressionSlider;
    [SerializeField] private Explosives explosives;

    #endregion

    #region Private State

    private float        delayedFillValue;
    private float        delayTimer;
    private float        previousHealth;
    private CanvasGroup  staminaGroup;
    private HealthSystem healthSystem;
    private Coroutine    messageCoroutine;

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

        if (interactLabel          != null) interactLabel.enabled          = false;
        if (messageText            != null) messageText.enabled            = false;
        if (holdProgressRing       != null) holdProgressRing.enabled       = false;
        if (weaponHoldProgressRing != null)
        {
            weaponHoldProgressRing.enabled    = false;
            weaponHoldProgressRing.fillAmount = 0f;
        }
        if (explosivesProgressionSliderGameObject != null)
            explosivesProgressionSliderGameObject.SetActive(false);
    }

    private void Start()
    {
        if (playerController != null)
        {
            healthSystem = playerController.GetComponent<HealthSystem>();
            if (healthSystem != null)
                healthSystem.OnHealthChanged += OnHealthChanged;

            delayedFillValue = playerController.HealthNormalised;
            previousHealth   = playerController.HealthNormalised;
            SetDelayedRect(delayedFillValue);
        }

        questionMarkEmpty.gameObject.SetActive(false);
        questionMarkFill.gameObject.SetActive(false);
        exclamationMark.gameObject.SetActive(false);
        AlertSystem.Instance.OnAlertTriggered += Exclamation;
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

        if (enemyAwareness != null)
            UpdateEnemySuspicion();

        UpdateExplosivesProgression();
    }

    #endregion

    #region Health

    private void OnHealthChanged(float newHealth)
    {
        float normalised = healthSystem.HealthNormalised;
        if (normalised < previousHealth - 0.001f)
            delayTimer = delayBeforeDrop;
        previousHealth = normalised;
    }

    private void UpdateHealthBar()
    {
        if (healthSlider == null) return;
        float realHealth   = healthSystem.HealthNormalised;
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
        ammoText.color = bullets <= Mathf.CeilToInt(capacity * 0.3f) ? ammoColorLow : ammoColorNormal;
    }

    #endregion

    #region Interact Label

    public void SetInteractLabel(string text)
    {
        if (interactLabel == null) return;
        interactLabel.enabled = !string.IsNullOrEmpty(text);
        interactLabel.text    = text ?? "";
    }

    #endregion

    #region Hold Progress

    // progress: 0-1 · visible: si se muestra o no
    public void SetHoldProgress(float progress, bool visible)
    {
        if (holdProgressRing == null) return;
        holdProgressRing.enabled    = visible;
        holdProgressRing.fillAmount = progress;
    }

    // Radial específico para "sacar el arma" — separado del de interacción
    // para evitar que InteractionSystem lo pise cuando no hay target.
    public void SetWeaponHoldProgress(float progress, bool visible)
    {
        if (weaponHoldProgressRing == null) return;
        weaponHoldProgressRing.enabled    = visible;
        weaponHoldProgressRing.fillAmount = progress;
    }

    #endregion

    #region Message

    public void ShowMessage(string text, float duration)
    {
        if (messageText == null) return;
        if (messageCoroutine != null) StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(MessageRoutine(text, duration));
    }

    private IEnumerator MessageRoutine(string text, float duration)
    {
        messageText.text    = text;
        messageText.enabled = true;
        yield return new WaitForSeconds(duration);
        messageText.enabled = false;
        messageCoroutine    = null;
    }

    #endregion

    #region Public API

    public void Setup(PlayerController player, GunSystem gun)
    {
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


    public static EnemyAwarenessComponent enemyAwareness;

    public static void UpdateMostAwareEnemy(EnemyAwarenessComponent awareness)
    {
        if (enemyAwareness == null)
        {
            enemyAwareness = awareness;
            return;
        }
        if (enemyAwareness.GetAwareness() < awareness.GetAwareness()) { enemyAwareness = awareness; }

        
    }

    private void UpdateEnemySuspicion()
    {
        enemySuspicion.text = Mathf.FloorToInt(enemyAwareness.GetAwareness()).ToString();

        questionMarkEmpty.gameObject.SetActive(true);
        questionMarkFill.gameObject.SetActive(true);

        questionMarkFill.fillAmount = enemyAwareness.GetAwareness() / 100;

        if (enemyAwareness.GetAwareness() == 0)
        {
            enemyAwareness = null;
            enemySuspicion.text = "";

            questionMarkEmpty.gameObject.SetActive(false);
            questionMarkFill.gameObject.SetActive(false);
        }
    }

    private void UpdateExplosivesProgression()
    {
        if (explosives.GetProgress() == 0) return;
        explosivesProgressionSliderGameObject.SetActive(explosives.inProgress);
        explosivesProgressionSlider.enabled = true;
        explosivesProgressionSlider.value = explosives.GetProgress();
    }

    public void Exclamation()
    {
        exclamationMark.gameObject.SetActive(!(exclamationMark.gameObject.activeSelf));
        StartCoroutine(Corutine());
    }

    IEnumerator Corutine()
    {
        yield return new WaitForSeconds(exclamationMarkTime);

        exclamationMark.gameObject.SetActive(!(exclamationMark.gameObject.activeSelf));
    }
}