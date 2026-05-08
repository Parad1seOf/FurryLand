using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] GameObject gameResultsUI;

    private bool canExit = false;

    void Start()
    {
        if (gameResultsUI != null) gameResultsUI.SetActive(false);
    }

    void Update()
    {
        //canExit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (canExit && other.CompareTag("Player")) FinishGame();
        if (ScoreManager.instance.HasFurryConstitution())
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        if (gameResultsUI != null)
        {
           gameResultsUI.SetActive(true);
        }

        GameResultUI resultsUI = FindFirstObjectByType<GameResultUI>();

        if (resultsUI != null) resultsUI.ShowResults(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        /*Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);*/
    }
}
