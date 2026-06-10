using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Phase2FXManager : MonoBehaviour
{
    public static Phase2FXManager instance { get; private set; }

    public static bool ProfileAlreadyChosenStatic { get; private set; } = false;
    public static int SelectedProfileIndex { get; private set; } = 1;

    [Header("References")]
    [SerializeField] private Volume phase2Volume;
    [SerializeField] private Volume healthVolume;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Profiles")]
    [SerializeField] private VolumeProfile option1;
    [SerializeField] private VolumeProfile option2;
    [SerializeField] private GameObject postProcessSelectorCanvas;

    [Header("Damage Configuration")]
    [SerializeField] private float increaseAmount = 0.2f;
    [SerializeField] private float decaySpeed = 0.25f;
    [SerializeField] private float delay = 3f;

    private bool phase2Active = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (ProfileAlreadyChosenStatic)
        {
            DestroyFadeBug();
            if (postProcessSelectorCanvas != null) postProcessSelectorCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
        else
            StartSelectorMenu();

        ComponentsConfiguration();
    }

    void LateUpdate()
    {
        if (!ProfileAlreadyChosenStatic && postProcessSelectorCanvas != null && postProcessSelectorCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (phase2Volume != null)
        {
            bool active = phase2Active;
            if (phase2Volume.gameObject.activeSelf != active)
                phase2Volume.gameObject.SetActive(active);

            phase2Volume.weight = active ? 1f : 0f;
        }
    }

    public void ChooseProfile1()
    {
        if (phase2Volume != null && option1 != null) phase2Volume.profile = option1;

        ProfileAlreadyChosenStatic = true;
        SelectedProfileIndex = 1;
        GoToIntro();
    }

    public void ChooseProfile2()
    {
        if (phase2Volume != null && option2 != null) phase2Volume.profile = option2;

        ProfileAlreadyChosenStatic = true;
        SelectedProfileIndex = 2;
        GoToIntro();
    }

    private void GoToIntro()
    {
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(1);
        else
            SceneManager.LoadScene(1);
    }

    private void StartSelectorMenu()
    {
        Time.timeScale = 0.0001f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (postProcessSelectorCanvas != null)
            postProcessSelectorCanvas.SetActive(true);

        TogglePlayerObject(false);
    }

    private void TogglePlayerObject(bool enabled)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
            player.gameObject.SetActive(enabled);
    }

    private void ComponentsConfiguration()
    {
        if (phase2Volume != null && ProfileAlreadyChosenStatic)
            phase2Volume.profile = SelectedProfileIndex == 1 ? option1 : option2;

        if (healthVolume != null) healthVolume.weight = 0f;
        if (healthSystem == null) healthSystem = FindFirstObjectByType<HealthSystem>();

        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthFX;
            healthSystem.OnHealthChanged += UpdateHealthFX;
        }

        if (AlertSystem.Instance != null)
        {
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
            AlertSystem.Instance.OnAlertTriggered += EnablePhase2FX;
        }
    }

    private void DestroyFadeBug()
    {
        GameObject fadeBug = GameObject.Find("CanvasFade");
        if (fadeBug != null) Destroy(fadeBug);
    }

    private void EnablePhase2FX()
    {
        phase2Active = true;
    }

    private void UpdateHealthFX(float amount)
    {
        if (healthVolume != null && healthSystem != null)
            healthVolume.weight = 1 - healthSystem.HealthNormalised;
    }
}