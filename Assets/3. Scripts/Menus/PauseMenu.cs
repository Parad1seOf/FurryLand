using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] bool gamePaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    public bool isPaused { get { return gamePaused; }}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            if (gamePaused) Continue();

            else Pause();
        }
    }

    public void Pause()
    {
        gamePaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()
    {
        gamePaused = false;
        
        pauseMenuUI.SetActive(false);
        if(optionsPanel != null) optionsPanel.SetActive(false);
        if(controlsPanel != null) controlsPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ShowOptions(bool show) => optionsPanel.SetActive(show);
    public void ShowControls(bool show) => controlsPanel.SetActive(show);
}
