using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] GameObject gameResultsUI;

    void Start()
    {
        if (gameResultsUI != null) gameResultsUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ScoreManager.instance.HasFurryConstitution())
            {
                FinishGame();
            }
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
}
