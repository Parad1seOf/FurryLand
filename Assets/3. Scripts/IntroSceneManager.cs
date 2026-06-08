using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private float cinematicDuration = 10f;

    void Start()
    {
        Invoke(nameof(FinishCinematic), cinematicDuration);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke(nameof(FinishCinematic));
            FinishCinematic();
        }
    }

    private void FinishCinematic()
    {
        if (FadeManager.Instance != null)
        {
            if (MainMenu.FromExtras)
                FadeManager.Instance.ChangeSceneFade(0);
            else
                FadeManager.Instance.ChangeSceneFade(2);
        }
        else
        {
            if (MainMenu.FromExtras) SceneManager.LoadScene(0);
            else SceneManager.LoadScene(2);
        }
    }
}