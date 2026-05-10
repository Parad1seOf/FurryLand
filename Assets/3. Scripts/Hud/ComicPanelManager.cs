using TMPro;
using UnityEngine;

public class ComicPanelManager : MonoBehaviour
{
    public static ComicPanelManager Instance;

    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI panelText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if(container != null)
            container.SetActive(false);
    }
    
    public void ShowPhrase(string phrase, float duration = 4f)
    {
        if (container == null || panelText == null) return;

        panelText.text = phrase;
        container.SetActive(true);

        CancelInvoke(nameof(HidePanel));
        Invoke(nameof(HidePanel), duration);
    }

    private void HidePanel() => container.SetActive(false);
}
