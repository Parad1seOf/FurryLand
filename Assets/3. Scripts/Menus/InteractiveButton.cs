using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject colorFilter; 
    [SerializeField] float increasedScale = 1.05f;
    [SerializeField] float speed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * increasedScale;

        if (colorFilter != null) colorFilter.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        if (colorFilter != null) colorFilter.SetActive(true);
    }
}
