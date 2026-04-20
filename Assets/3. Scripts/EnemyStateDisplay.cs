using UnityEngine;
using TMPro;

public class EnemyStateDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStateMachine stateMachine;
    [SerializeField] private TextMeshPro label;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);
    [SerializeField] private bool faceCamera = true;

    private Camera cam;

    private void Awake()
    {
        if (stateMachine == null)
            stateMachine = GetComponentInParent<EnemyStateMachine>();

        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (label == null || stateMachine == null) return;

        // Posición sobre la cabeza
        label.transform.position = transform.position + offset;

        // Siempre mira a la cámara
        if (faceCamera && cam != null)
            label.transform.forward = cam.transform.forward;

        // Texto + sospecha
        /*string stateName = stateMachine.CurrentStateName;
        float  sus       = stateMachine.SuspicionLevel;

        label.text = sus > 0f
            ? $"{stateName}\n<size=70%>{sus:F0} / 100</size>"
            : stateName;

        // Color según estado
        label.color = stateName switch
        {
            nameof(EnemyIdleState)      => Color.white,
            nameof(EnemySuspicionState) => Color.yellow,
            nameof(EnemyAlertState)     => Color.red,
            _                           => Color.white
        };*/
    }
}