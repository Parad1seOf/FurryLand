using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private float cinematicDuration = 94.2f;

    [Header("Postprocessing references")]
    [SerializeField] private Volume introVolume;
    [SerializeField] private VolumeProfile profileOne;
    [SerializeField] private VolumeProfile profileTwo;

    private bool finishing = false;

    void Start()
    {
        ApplySelectedFilter();

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

    private void ApplySelectedFilter()
    {
        if (introVolume == null) return;

        if (Phase2FXManager.ProfileAlreadyChosenStatic)
            introVolume.profile = Phase2FXManager.SelectedProfileIndex == 1 ? profileOne : profileTwo;

        else
            introVolume.profile = profileOne;

        introVolume.gameObject.SetActive(true);
        introVolume.weight = 1f;
    }

    private void FinishCinematic()
    {
        if (finishing) return;
        finishing = true;
        Time.timeScale = 1f;

        if (FadeManager.Instance != null)
        {
            if (MainMenu.FromExtras) FadeManager.Instance.ChangeSceneFade(0);
            else FadeManager.Instance.ChangeSceneFade(2);
        }
        else
        {
            if (MainMenu.FromExtras) SceneManager.LoadScene(0);
            else SceneManager.LoadScene(2);
        }
    }
}