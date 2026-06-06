using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] bool gamePaused = false;

    [Header("Main Canvas")]
    [SerializeField] GameObject pauseMenuUI;

    [Header("Panels")]
    [SerializeField] private GameObject pauseVisualContent;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    public bool isPaused { get { return gamePaused; }}

    private void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (pauseVisualContent != null) pauseVisualContent.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (gamePaused) Continue();

            else Pause();
        }
    }

    public void Pause()
    {
        gamePaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (pauseVisualContent != null) pauseVisualContent.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        AudioListener.pause = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()
    {
        gamePaused = false;
        AudioListener.pause = false;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (pauseVisualContent != null) pauseVisualContent.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        ForbiddenArea.ResetComicTrigger();
        AmmoItem.ResetAmmoTrigger();
        Spawner.ResetParkourTrigger();
        SceneManager.LoadScene(0);
    }

    public void ShowOptions(bool show)
    {
        if (optionsPanel != null) optionsPanel.SetActive(show);
        if (pauseVisualContent != null) pauseVisualContent.SetActive(!show);
    }

    public void ShowControls(bool show)
    {
        if (controlsPanel != null) controlsPanel.SetActive(show);
        if (pauseVisualContent != null) pauseVisualContent.SetActive(!show);
    }
}
