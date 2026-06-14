using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject extrasPanel;
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private GameObject endingCinematicButton;

    public static bool FromExtras = false;
    public static bool GameCompletedInSession = false;

    [Header("Options components")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Toggle fogToggle;

    public static int PostProcessingChoice = 0;

    private static float SavedVolume = 50f;
    private static float SavedFOV = 90f;
    private static bool SavedFog = true;
    private static bool OptionsAlreadyInitialized = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (FromExtras)
        {
            FromExtras = false;
            if (extrasPanel != null) extrasPanel.SetActive(true);
        }
        else
        {
            if (extrasPanel != null) extrasPanel.SetActive(false);
        }

        if (endingCinematicButton != null)
            endingCinematicButton.SetActive(GameCompletedInSession);

        if (!OptionsAlreadyInitialized)
        {
            if (volumeSlider != null) SavedVolume = volumeSlider.value;
            if (FOVSlider != null) SavedFOV = FOVSlider.value;
            if (fogToggle != null) SavedFog = fogToggle.isOn;
            OptionsAlreadyInitialized = true;
        }
        else
        {
            if (volumeSlider != null) volumeSlider.value = SavedVolume;
            if (FOVSlider != null) FOVSlider.value = SavedFOV;
            if (fogToggle != null) fogToggle.isOn = SavedFog;
        }

        ApplyAllSettings();
    }

    public void ChangeVolume(float val)
    {
        SavedVolume = val;
        ApplyAllSettings();
    }

    public void ChangeFOV(float val)
    {
        SavedFOV = val;
        ApplyAllSettings();
    }

    public void ChangeFog(bool val)
    {
        SavedFog = val;
        ApplyAllSettings();
    }

    private void ApplyAllSettings()
    {
        AudioListener.volume = SavedVolume;
        RenderSettings.fog = SavedFog;

        if (Camera.main != null)
            Camera.main.fieldOfView = SavedFOV;
    }

    #region Navigation

    public void Play()
    {
        MainMenu.FromExtras = false;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(2);
        else
            SceneManager.LoadScene(2);
    }

    public void OnPostProcessingSelected(int choice)
    {
        PostProcessingChoice = choice;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(1);
        else
            SceneManager.LoadScene(1);
    }

    public void Exit() => Application.Quit();

    #endregion

    #region Panel Management
    public void ShowControls(bool show) => controlsPanel.SetActive(show);
    public void ShowOptions(bool show) => optionsPanel.SetActive(show);
    public void ShowExtras(bool show) => extrasPanel.SetActive(show);
    public void ShowCredits(bool show) => creditsPanel.SetActive(show);
    #endregion

    #region Post Processing
    public void SelectFXOption1() => PostProcessingChoice = 0;
    public void SelectFXOption2() => PostProcessingChoice = 1;
    #endregion

    #region Cinematics
    public void ReproduceIntroCinematic()
    {
        FromExtras = true;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(1);

        else
            SceneManager.LoadScene(1);
    }

    public void ReproduceEndingCinematic()
    {
        if (!GameCompletedInSession) return;

        FromExtras = true;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(3);
        else
            SceneManager.LoadScene(3);
    }
    #endregion
}