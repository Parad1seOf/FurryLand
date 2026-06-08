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

    [Header("Options components")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Toggle fogToggle;

    public static int PostProcessingChoice = 0;

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
        {
            bool gameCompleted = PlayerPrefs.GetInt("GameCompleted", 0) == 1;
            endingCinematicButton.SetActive(gameCompleted);
        }
    }

    #region Navigation

    public void Play()
    {
        FromExtras = false;

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

    public void ReproduceEndingCinematic() { }
    #endregion
}