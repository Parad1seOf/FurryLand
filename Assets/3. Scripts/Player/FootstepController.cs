using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepController : MonoBehaviour
{
    public AudioManager audioManager;

    private CharacterController cc;
    private bool isPlaying;

    private void Awake() => cc = GetComponent<CharacterController>();

    // Llama desde PlayerController cada Update
    public void Tick(bool isMoving)
    {
        if (audioManager == null) return;

        if (cc.isGrounded && isMoving)
        {
            if (!isPlaying) { audioManager.Walking(); isPlaying = true; }
        }
        else
        {
            audioManager.StopWalking();
            isPlaying = false;
        }
    }
}