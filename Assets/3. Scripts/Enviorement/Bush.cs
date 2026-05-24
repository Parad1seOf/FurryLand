using UnityEngine;

public class Bush : MonoBehaviour, ISpeedModifier
{
    [SerializeField] private float speedReduction = 5f;

    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.AddSpeedModifier(this);
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.RemoveSpeedModifier(this);
    }

    public float GetValue() {  return -speedReduction; }
}
