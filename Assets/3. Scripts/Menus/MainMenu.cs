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

    [Header("Options components")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Toggle fogToggle;

    private void Start()
    {
        Time.timeScale = 1f;

        if(controlsPanel != null) controlsPanel.SetActive(false);
        if(optionsPanel != null) optionsPanel.SetActive(false);
        if(extrasPanel != null) extrasPanel.SetActive(false);

        LoadConfig();

        if(endingCinematicButton != null)
        {
            bool gameCompleted = PlayerPrefs.GetInt("GameCompleted", 0) == 1;
            endingCinematicButton.SetActive(gameCompleted);
        }
    }

    #region Navigation

    public void Play()
    {
        // Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.FadeToScene("Testing Impulso");

        else
            SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion

    #region Panel Management
    public void ShowControls(bool show) => controlsPanel.SetActive(show);
    public void ShowOptions(bool show) => optionsPanel.SetActive(show);
    public void ShowExtras(bool show) => extrasPanel.SetActive(show);
    public void ShowCredits(bool show) => creditsPanel.SetActive(show);

    #endregion

    #region Options Logic
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("GeneralVolume", value);
    }

    public void ChangeFOV(float value)
    {
        PlayerPrefs.SetFloat("FOVConfig", value);
    }

    public void ChangeFog(bool activate)
    {
        RenderSettings.fog = activate;
        PlayerPrefs.SetInt("FogConfig", activate ? 1 : 0);
    }

    private void LoadConfig()
    {
        float vol = PlayerPrefs.GetFloat("GeneralVolume", 1f);
        if(volumeSlider != null) volumeSlider.value = vol;
        AudioListener.volume = vol;

        float fov = PlayerPrefs.GetFloat("FOVConfig", 60f);
        if(FOVSlider != null) FOVSlider.value = fov;

        bool fogActive = PlayerPrefs.GetInt("FogConfig", 1) == 1;
        if(fogToggle != null) fogToggle.isOn = fogActive;
        RenderSettings.fog = fogActive;
    }
    #endregion

    #region Cinematics
    public void ReproduceInitialCinematic()
    {

    }

    public void ReproduceEndingCinematic()
    {

    }
    #endregion
}
