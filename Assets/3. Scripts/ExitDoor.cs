using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] GameObject gameResultsUI;

    private bool canExit = false;
    private float timer = 0f;

    void Start()
    {
        if (gameResultsUI != null) gameResultsUI.SetActive(false);
    }

    void Update()
    {
        /* if (!canExit)
         {
             //timer += Time.deltaTime;
             canExit = true;
         }*/
        canExit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canExit && other.CompareTag("Player")) FinishGame();
    }

    private void FinishGame()
    {
        GameResultUI resultsUI = FindFirstObjectByType<GameResultUI>();

        if (resultsUI != null) resultsUI.ShowResults(true);
    }
}
