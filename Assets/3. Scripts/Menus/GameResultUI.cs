using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] Image resultImage;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI numberOfKills;
    [SerializeField] TextMeshProUGUI totalTime;
    [SerializeField] TextMeshProUGUI shootingAccuracy;
    [SerializeField] TextMeshProUGUI totalReceivedDamage;

    [Header("Result sprites")]
    [SerializeField] Sprite victorySprite;
    [SerializeField] Sprite rabbitKillerSprite;
    [SerializeField] Sprite sniperKillerSprite;

    [Header("Clips — Victory")]
    [SerializeField] private AudioSource localAudioSource;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("Buttons Configuration")]
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject watchEndingButton;


    public void ShowResults(bool isVictory, string killerType = "")
    {
        mainPanel.SetActive(true);
        resultText.text = isVictory ? "VICTORIA":"DERROTA";

        if (retryButton != null) retryButton.SetActive(false);
        if (watchEndingButton != null) watchEndingButton.SetActive(false);

        if (isVictory)
        {
            if (watchEndingButton != null) watchEndingButton.SetActive(true);
            MainMenu.GameCompletedInSession = true;
        }

        else
        {
            if (retryButton != null) retryButton.SetActive(true);
        }

        if (isVictory)
            resultImage.sprite = victorySprite;

        else
        {
            if (killerType == "Rabbit")
                resultImage.sprite = rabbitKillerSprite;
            else
                resultImage.sprite = sniperKillerSprite;
        }

        AudioListener.pause = true;

        if (localAudioSource != null)
        {
            localAudioSource.ignoreListenerPause = true;

            AudioClip clipToPlay = isVictory ? victoryMusic : defeatMusic;
            if (clipToPlay != null)
            {
                localAudioSource.clip = clipToPlay;
                localAudioSource.Play();
            }
        }

        int kills = ScoreManager.instance.GetKillCount();
        int minutes = Timer.instance.GetMinutes();
        int seconds = Timer.instance.GetSeconds();
        int accuracy = ScoreManager.instance.GetAccuracy();

        float damage = 0f;
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                    damage = playerHealth.totalDamage;
            }
        }

        numberOfKills.text = "Kills: " + kills;
        totalTime.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
        shootingAccuracy.text = "Precisión: " + accuracy + "%";
        totalReceivedDamage.text = "Daño recibido: " + Mathf.RoundToInt(damage) + " HP";

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBirdsAmbience();

        ForbiddenArea.ResetComicTrigger();
        AmmoItem.ResetAmmoTrigger();
        Spawner.ResetParkourTrigger();


        if (FadeManager.Instance != null)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            FadeManager.Instance.ChangeSceneFade(currentSceneIndex);
        }
        
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WatchEnding()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (FadeManager.Instance != null)
            FadeManager.Instance.ChangeSceneFade(3);

        else
            SceneManager.LoadScene(3);
    }
}
