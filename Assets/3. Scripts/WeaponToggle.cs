using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponToggle : MonoBehaviour
{
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private float suspiciousness;

    [Header("UI Crosshair")]
    [SerializeField] private GameObject crosshairObject;

    [Header("Hold to Draw")]
    [Tooltip("Segundos que hay que mantener pulsada la tecla del arma para sacarla.")]
    [SerializeField] private float drawHoldTime = 1.2f;
    [Tooltip("HUDManager para mostrar el radial. Si se deja vacío, se busca en escena.")]
    [SerializeField] private HUDManager hudManager;

    private InputReader input;
    private float drawTimer;

    private bool hasTriggeredComicPanel = false;
    private bool drawn = false;
    public bool IsWeaponDrawn => drawn;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        Holster();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Holster();
    }

    private void Start()
    {
        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered += Alarm;

        if (hudManager == null)
            hudManager = Object.FindFirstObjectByType<HUDManager>();
    }

    private void OnDestroy()
    {
        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= Alarm;
    }

    private void Holster()
    {
        drawn = false;
        weaponObject?.SetActive(false);
        drawTimer = 0f;
        if (hudManager != null) hudManager.SetWeaponHoldProgress(0f, false);

        if (crosshairObject != null) crosshairObject.SetActive(false);
    }

    private void Update()
    {
        if (IsWeaponDrawn)
        {
            if (drawTimer != 0f)
            {
                drawTimer = 0f;
                hudManager?.SetWeaponHoldProgress(0f, false);
            }
            return;
        }

        if (input == null) return;
        if (input.WeaponHeld)
        {
            drawTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(drawTimer / Mathf.Max(0.0001f, drawHoldTime));
            hudManager?.SetWeaponHoldProgress(progress, true);

            if (drawTimer >= drawHoldTime)
            {
                Toggle();
                drawTimer = 0f;
                hudManager?.SetWeaponHoldProgress(0f, false);
            }
        }
        else
        {
            if (drawTimer > 0f)
            {
                drawTimer = 0f;
                hudManager?.SetWeaponHoldProgress(0f, false);
            }
        }
    }

    private void Toggle()
    {
        drawn = true;
        weaponObject?.SetActive(true);

        if (crosshairObject != null) crosshairObject.SetActive(true);

        SuspicionComponent sus = GetComponent<SuspicionComponent>();
        if (weaponObject != null && weaponObject.activeSelf) sus?.RiseSuspicion(suspiciousness);
        else sus?.LowerSuspicion(suspiciousness);

        if (!hasTriggeredComicPanel && AlertSystem.Instance != null && !AlertSystem.Instance.IsAlreadyTriggered)
        {
            hasTriggeredComicPanel = true;

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.ShowPhraseByID("Weapon_Stealth");
        }
    }

    public void Alarm()
    {
        Toggle();
    }
}