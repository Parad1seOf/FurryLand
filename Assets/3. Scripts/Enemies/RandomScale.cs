using UnityEngine;

public class RandomScale : MonoBehaviour
{
    [SerializeField] private float minScale = 1.3f;
    [SerializeField] private float maxScale = 1.5f;

    private void Awake()
    {
        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * randomScale;
    }
}