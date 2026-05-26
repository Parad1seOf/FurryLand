using UnityEngine;

public class FurryConstitution : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            ScoreManager.instance.ObtainFurryConstitution();

            if (ComicPanelManager.Instance != null)
                ComicPanelManager.Instance.ShowPhraseByID("FurryConstitucion_Pickup");

            gameObject.SetActive(false);
        }
    }
}
