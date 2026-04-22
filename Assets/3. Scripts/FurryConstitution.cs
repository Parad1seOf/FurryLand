using UnityEngine;

public class FurryConstitution : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            ScoreManager.instance.ObtainFurryConstitution();
            gameObject.SetActive(false);
        }
    }
}
