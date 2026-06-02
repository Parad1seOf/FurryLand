using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro label;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image questionMarkEmpty;
    [SerializeField] private Image questionMarkFill;
    [SerializeField] private Image exclamationMark;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);
    [SerializeField] private bool faceCamera = true;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        questionMarkEmpty.gameObject.SetActive(false);
        questionMarkFill.gameObject.SetActive(false);
        exclamationMark.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {

        // Posición sobre la cabeza
        label.transform.position = transform.position + offset;

        // Siempre mira a la cámara
        if (faceCamera && cam != null)
            label.transform.forward = cam.transform.forward;

        if (faceCamera && cam != null)
            canvas.transform.forward = cam.transform.forward;
    }

    public void ChangeLabel(string str, Color color)
    {
        label.text = str;
        label.color = color;
    }

    public void ShowSuspicion(float sus)
    {
        questionMarkEmpty.gameObject.SetActive(true);
        questionMarkFill.gameObject.SetActive(true);

        questionMarkFill.fillAmount = sus / 100;
    }

    public void HideSuspicion()
    {
        questionMarkEmpty.gameObject.SetActive(false);
        questionMarkFill.gameObject.SetActive(false);
    }

    public void Exclamation()
    {
        exclamationMark.gameObject.SetActive(!(exclamationMark.gameObject.activeSelf));
    }
}
