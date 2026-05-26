using UnityEngine;

public class ExitApproachTrigger : MonoBehaviour
{
    private bool hasTriggeredComicPanel = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && !hasTriggeredComicPanel)
        {
            if (ScoreManager.instance != null && ScoreManager.instance.HasFurryConstitution())
            {
                hasTriggeredComicPanel = true;

                if (ComicPanelManager.Instance != null)
                    ComicPanelManager.Instance.ShowPhraseByID("Exit_Approach");
            }
        }
    }
}