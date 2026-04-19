//Gestiona los fades de escena (juego - muerte etc) y el hit flash del HUD.
// HUDManager le registra las referencias de UI en cada escena via RegisterUI().
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    #endregion

    #region Inspector Fields

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Hit Flash")]
    public Image hitFlashImage;
    public float hitFlashDuration = 0.2f;

    #endregion

    #region Private State

    private bool isFading      = false;
    private bool isFlashingHit = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FadeIn();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeIn();
    }

    #endregion

    #region Public API

    public void RegisterUI(Image fade, Image hitFlash)
    {
        fadeImage     = fade;
        hitFlashImage = hitFlash;
    }

    #endregion

    #region Scene Management

    public void FadeToScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(FadeToSceneRoutine(sceneName));
    }

    private IEnumerator FadeToSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeOutRoutine());
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return StartCoroutine(FadeInRoutine());
    }

    #endregion

    #region Screen Fades

    public void FadeIn()
    {
        if (!isFading) StartCoroutine(FadeInRoutine());
    }

    public void FadeOut()
    {
        if (!isFading) StartCoroutine(FadeOutRoutine());
    }

    public void FadeOutAndIn()
    {
        if (!isFading) StartCoroutine(FadeOutInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        if (isFading) yield break;
        isFading = true;

        for (float t = fadeDuration; t > 0f; t -= Time.deltaTime)
        {
            SetFadeAlpha(t / fadeDuration);
            yield return null;
        }
        SetFadeAlpha(0f);
        isFading = false;
    }

    private IEnumerator FadeOutRoutine()
    {
        if (isFading) yield break;
        isFading = true;

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            SetFadeAlpha(t / fadeDuration);
            yield return null;
        }
        SetFadeAlpha(1f);
        isFading = false;
    }

    private IEnumerator FadeOutInRoutine()
    {
        yield return StartCoroutine(FadeOutRoutine());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeInRoutine());
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    #endregion

    #region Hit Flash

    public void ShowHitFlash()
    {
        if (!isFlashingHit)
            StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        if (isFlashingHit) yield break;
        isFlashingHit = true;

        SetHitFlashAlpha(1f);
        for (float t = hitFlashDuration; t > 0f; t -= Time.deltaTime)
        {
            SetHitFlashAlpha(t / hitFlashDuration);
            yield return null;
        }
        SetHitFlashAlpha(0f);
        isFlashingHit = false;
    }

    private void SetHitFlashAlpha(float alpha)
    {
        if (hitFlashImage == null) return;
        Color c = hitFlashImage.color;
        c.a = alpha;
        hitFlashImage.color = c;
    }

    #endregion
}