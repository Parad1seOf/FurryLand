using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComicPanelManager : MonoBehaviour
{
    public static ComicPanelManager Instance { get; private set; }

    [Header("Base Configuration")]
    [SerializeField] private PhrasesData phrasesData;
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI panelText;
    [SerializeField] private float duration = 6f;
    [SerializeField] private float width = 600;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip appearSound;
    [SerializeField] private AudioClip disappearSound;

    [Header("Screen Limits")]
    [SerializeField] private float minDistanceFromEdge = 80f;
    [SerializeField] private float maxDistanceFromEdge = 350f;
    [SerializeField] private float maxTopY = 300f;
    [SerializeField] private float minBottomY = -400f;

    [Header("Paddings")]
    [SerializeField] private float horizontalPadding = 100f;
    [SerializeField] private float verticalPadding = 100f;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rectTransform = GetComponent<RectTransform>();

        if (container != null)
            container.SetActive(false);
    }

    public void ShowPhraseByID(string id, float duration = 0f)
    {
        if (duration <= 0) duration = this.duration;

        if (container == null || panelText == null || phrasesData == null) return;

        string displayText = phrasesData.GetPhrase(id);

        if (!string.IsNullOrEmpty(displayText))
        {
            panelText.text = displayText;
            container.SetActive(true);

            Adjust();

            if (appearSound != null && AudioManager.Instance != null)
                AudioManager.Instance.sfxSource.PlayOneShot(appearSound);

            CancelInvoke(nameof(HidePanel));
            Invoke(nameof(HidePanel), duration);
        }
    }

    public void StartC4Reminder(string id, float duration)
    {
        CancelInvoke(nameof(TriggerC4Reminder));
        Invoke(nameof(TriggerC4Reminder), duration);
    }

    public void CancelC4Reminder()
    {
        CancelInvoke(nameof(TriggerC4Reminder));
    }

    private void HidePanel()
    {
        if (disappearSound != null && AudioManager.Instance != null && container.activeSelf)
            AudioManager.Instance.sfxSource.PlayOneShot(disappearSound);

        container.SetActive(false);
    }

    private void TriggerC4Reminder()
    {
        ShowPhraseByID("C4_Checkpoint");
    }

    private void Adjust()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelText.rectTransform);

        float height = panelText.preferredHeight;
        rectTransform.sizeDelta = new Vector2(width, height);

        float leftColumnMinX = -960f + minDistanceFromEdge + (width / 2f);
        float leftColumnMaxX = -960f + maxDistanceFromEdge;
        float rightColumnMinX = 960f - maxDistanceFromEdge;
        float rightColumnMaxX = 960f - minDistanceFromEdge - (width / 2f);

        if (leftColumnMaxX < leftColumnMinX) leftColumnMaxX = leftColumnMinX;
        if (rightColumnMinX > rightColumnMaxX) rightColumnMinX = rightColumnMaxX;

        float finalX = 0f;
        float finalY = 0f;

        int side = UnityEngine.Random.Range(0, 2);

        if (side == 0)
            finalX = UnityEngine.Random.Range(leftColumnMinX, leftColumnMaxX);
        else
            finalX = UnityEngine.Random.Range(rightColumnMinX, rightColumnMaxX);

        float localMaxY = maxTopY - (height / 2f);
        float localMinY = minBottomY + (height / 2f);

        if (localMinY > localMaxY) localMinY = localMaxY;

        finalY = UnityEngine.Random.Range(localMinY, localMaxY);
        rectTransform.anchoredPosition = new Vector2(finalX, finalY);
    }
}