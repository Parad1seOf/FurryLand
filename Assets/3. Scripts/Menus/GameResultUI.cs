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
    [SerializeField] Sprite defeatSprite; //Esto habrá que cambiarlo más adelante

    [Header("Clips — Victoria")]
    [SerializeField] private AudioSource localAudioSource;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    //public static GameResultUI instance { get; private set; }

    public void ShowResults(bool isVictory)
    {
        mainPanel.SetActive(true);
        resultText.text = isVictory ? "VICTORIA":"DERROTA";
        resultImage.sprite = isVictory ? victorySprite : defeatSprite;

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

        if (FadeManager.Instance != null)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            FadeManager.Instance.ChangeSceneFade(currentSceneIndex);
        }
        
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
