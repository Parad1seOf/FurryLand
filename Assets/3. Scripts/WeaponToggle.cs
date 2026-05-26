using UnityEngine;

public class WeaponToggle : MonoBehaviour
{
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private float suspiciousness;

    [Header("Hold to Draw")]
    [Tooltip("Segundos que hay que mantener pulsada la tecla del arma para sacarla.")]
    [SerializeField] private float drawHoldTime = 1.2f;
    [Tooltip("HUDManager para mostrar el radial. Si se deja vacío, se busca en escena.")]
    [SerializeField] private HUDManager hudManager;

    private InputReader input;
    private float drawTimer;

    private bool hasTriggeredComicPanel = false;

    public bool IsWeaponDrawn => weaponObject != null && weaponObject.activeSelf;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        weaponObject?.SetActive(false); // empieza guardada
    }

    private void Start()
    {
        AlertSystem.Instance.OnAlertTriggered += Alarm;
        if (hudManager == null)
            hudManager = Object.FindFirstObjectByType<HUDManager>();
    }

    private void Update()
    {
        // Si el arma ya está sacada, no hay nada que rellenar.
        // (Se mantiene la funcionalidad original: una vez sacada se queda sacada,
        //  porque el Toggle() original siempre hace SetActive(true).)
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

        // Mantener la tecla → rellenar el radial
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
            // Se soltó la tecla antes de completar → cancelar, la próxima vez empieza desde 0
            if (drawTimer > 0f)
            {
                drawTimer = 0f;
                hudManager?.SetWeaponHoldProgress(0f, false);
            }
        }
    }

    private void Toggle()
    {
        weaponObject?.SetActive(true);
        SuspicionComponent sus = GetComponent<SuspicionComponent>();
        if (weaponObject.activeSelf) sus?.RiseSuspicion(suspiciousness);
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
        // La alarma saca el arma al instante, sin necesidad de mantener la tecla.
        Toggle();
    }
}