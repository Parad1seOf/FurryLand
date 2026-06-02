using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PhrasesData;
using static System.Net.Mime.MediaTypeNames;

public class ComicPanelManager : MonoBehaviour
{
    public static ComicPanelManager Instance { get; private set; }

    [SerializeField] private PhrasesData phrasesData;
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI panelText;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float width = 600;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        if(container != null)
            container.SetActive(false);
    }
    
    public void ShowPhraseByID(string id, float duration = 0f)
    {
        if (duration <= 0) duration = this.duration;

        if(container == null || panelText == null || phrasesData == null) return;

        string displayText = phrasesData.GetPhrase(id);

        if(!string.IsNullOrEmpty(displayText))
        {
            panelText.text = displayText;
            container.SetActive(true);
            Adjust();

            CancelInvoke(nameof(HidePanel));
            Invoke(nameof(HidePanel), duration);
        }
    }

    private void HidePanel() => container.SetActive(false);

    public void StartC4Reminder(string id, float duration)
    {
        CancelInvoke(nameof(TriggerC4Reminder));
        Invoke(nameof(TriggerC4Reminder), duration);
    }

    private void TriggerC4Reminder()
    {
        ShowPhraseByID("C4_Checkpoint");
    }

    public void CancelC4Reminder()
    {
        CancelInvoke(nameof(TriggerC4Reminder));
    }

    private void Adjust()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelText.rectTransform);

        float height = panelText.preferredHeight;

        GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }
}
