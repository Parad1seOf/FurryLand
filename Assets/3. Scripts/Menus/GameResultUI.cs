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
    [SerializeField] TextMeshProUGUI detectiveLevel;

    [Header("Result sprites")]
    [SerializeField] Sprite victorySprite;
    [SerializeField] Sprite defeatSprite; //Esto habrá que cambiarlo más adelante

    //public static GameResultUI instance { get; private set; }

    public void ShowResults(bool isVictory)
    {
        mainPanel.SetActive(true);
        resultText.text = isVictory ? "SUCCESS":"GAME OVER";
        resultImage.sprite = isVictory ? victorySprite : defeatSprite;


        int kills = ScoreManager.instance.GetKillCount();
        int minutes = Timer.instance.GetMinutes();
        int seconds = Timer.instance.GetSeconds();

        numberOfKills.text = "Kills: " + kills;
        totalTime.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        //Falta calcular el nivel de detective
        detectiveLevel.text = "Detective level: x%";

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
