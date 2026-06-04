using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Phase2FXManager : MonoBehaviour
{
    public static Phase2FXManager instance { get; private set; }

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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        StartSelectorMenu();
        ComponentsConfiguration();
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
        EndSelection();
    }

    public void ChooseProfile2()
    {
        if (phase2Volume != null && option2 != null) phase2Volume.profile = option2;
        EndSelection();
    }

    private void EndSelection()
    {
        if (postProcessSelectorCanvas != null) postProcessSelectorCanvas.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AudioListener.pause = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;

        phase2Active = false;

        StartSelectorMenu();
        ComponentsConfiguration();
    }

    private void StartSelectorMenu()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AudioListener.pause = true;

        if (postProcessSelectorCanvas != null)
            postProcessSelectorCanvas.SetActive(true);
    }

    private void ComponentsConfiguration()
    {
        if (phase2Volume != null)
        {
            phase2Volume.gameObject.SetActive(false);
            phase2Volume.weight = 0f;
        }

        if (healthVolume != null)
            healthVolume.weight = 0f;

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

    private void EnablePhase2FX()
    {
        phase2Active = true;
    }

    private void UpdateHealthFX(float amount)
    {
        if (healthVolume != null)
        {
            healthVolume.weight = 1 - healthSystem.HealthNormalised;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (healthSystem != null)
            healthSystem.OnHealthChanged -= UpdateHealthFX;

        if (AlertSystem.Instance != null)
            AlertSystem.Instance.OnAlertTriggered -= EnablePhase2FX;
    }
}
