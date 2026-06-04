using UnityEngine;

public class ForbiddenArea : MonoBehaviour
{
    [SerializeField]
    private float suspicionValue;

    private static bool hasTriggeredComic = false;

    private void OnTriggerEnter(Collider other)
    {
        SuspicionComponent sus = other.GetComponent<SuspicionComponent>();
        if (sus == null) return;

        sus.RiseSuspicion(suspicionValue);

        if (!hasTriggeredComic && AlertSystem.Instance != null && !AlertSystem.Instance.IsAlreadyTriggered)
        {
            hasTriggeredComic = true;

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.ShowPhraseByID("Enter_ForbiddenArea");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SuspicionComponent sus = other.GetComponent<SuspicionComponent>();
        if (sus == null) return;

        sus.LowerSuspicion(suspicionValue);
    }

    public static void ResetComicTrigger() => hasTriggeredComic = false;
}
