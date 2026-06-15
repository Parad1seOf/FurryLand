using UnityEngine;

public class MuteWhileCanvasVisible : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;

    private bool lastState;

    private void Awake()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponent<Canvas>();

        UpdateMuteState();
    }

    private void Update()
    {
        bool currentState = targetCanvas != null &&
                            targetCanvas.gameObject.activeInHierarchy &&
                            targetCanvas.enabled;

        if (currentState != lastState)
        {
            UpdateMuteState();
        }
    }

    private void UpdateMuteState()
    {
        bool shouldMute = targetCanvas != null &&
                          targetCanvas.gameObject.activeInHierarchy &&
                          targetCanvas.enabled;

        AudioListener.pause = shouldMute;

        lastState = shouldMute;
    }

    private void OnDisable()
    {
        // Por seguridad, al desactivar este objeto se restaura el audio.
        AudioListener.pause = false;
    }

    private void OnDestroy()
    {
        // También por seguridad.
        AudioListener.pause = false;
    }
}