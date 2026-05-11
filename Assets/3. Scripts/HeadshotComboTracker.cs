using UnityEngine;

public class HeadshotComboTracker : MonoBehaviour
{
    [Header("Combo")]
    [Tooltip("Segundos para encadenar el siguiente headshot antes de resetear.")]
    [SerializeField] private float comboWindow = 3f;
    [Tooltip("Combo máximo. Más allá de este valor el contador se clampea.")]
    [SerializeField] private int maxCombo = 5;
    [Tooltip("A partir de qué combo se muestra el mensaje. 2 = empieza por 'x2'.")]
    [SerializeField] private int minComboToShow = 2;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Clips por nivel de combo. Índice 0 = primer headshot, índice 4 = quinto.")]
    [SerializeField] private AudioClip[] comboClips = new AudioClip[5];

    [Header("HUD")]
    [SerializeField] private HUDManager hud;
    [SerializeField] private float messageDuration = 1.2f;

    private int currentCombo;
    private float comboTimer;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (hud == null) hud = FindFirstObjectByType<HUDManager>();
    }

    private void OnEnable()
    {
        AudioManager.OnHeadshotPerformed += HandleHeadshot;
    }

    private void OnDisable()
    {
        AudioManager.OnHeadshotPerformed -= HandleHeadshot;
    }

    private void Update()
    {
        if (currentCombo <= 0) return;

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f) ResetCombo();
    }

    private void HandleHeadshot()
    {
        currentCombo = Mathf.Min(currentCombo + 1, maxCombo);
        comboTimer = comboWindow;

        PlayComboSound(currentCombo);
        ShowComboMessage(currentCombo);
    }

    private void PlayComboSound(int combo)
    {
        if (audioSource == null || comboClips == null) return;

        int index = combo - 1;
        if (index < 0 || index >= comboClips.Length) return;
        if (comboClips[index] == null) return;

        audioSource.PlayOneShot(comboClips[index]);
    }

    private void ShowComboMessage(int combo)
    {
        if (hud == null || combo < minComboToShow) return;
        hud.ShowMessage($"x{combo}", messageDuration);
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        comboTimer = 0f;
    }
}