using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private float cinematicDuration = 130f;

    void Start()
    {
        Invoke(nameof(FinishCinematic), cinematicDuration);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke(nameof(FinishCinematic));
            FinishCinematic();
        }
    }

    private void FinishCinematic()
    {
        MainMenu.GameCompletedInSession = true;
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(0);

        else
            SceneManager.LoadScene(0);
    }
}