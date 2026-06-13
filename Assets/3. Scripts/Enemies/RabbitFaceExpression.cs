using UnityEngine;

public class RabbitFaceExpression : MonoBehaviour
{
    [Header("Renderer de la cabeza")]
    [SerializeField] private Renderer headRenderer;

    [Header("Materiales")]
    [SerializeField] private Material normalFaceMaterial;
    [SerializeField] private Material angryFaceMaterial;

    [Header("Índice del material de la cara")]
    [SerializeField] private int faceMaterialIndex = 0;

    private void Awake()
    {
        if (headRenderer != null && normalFaceMaterial == null)
        {
            Material[] mats = headRenderer.materials;

            if (faceMaterialIndex >= 0 && faceMaterialIndex < mats.Length)
                normalFaceMaterial = mats[faceMaterialIndex];
        }
    }

    public void SetAngryFace()
    {
        SetFaceMaterial(angryFaceMaterial);
    }

    public void SetNormalFace()
    {
        SetFaceMaterial(normalFaceMaterial);
    }

    private void SetFaceMaterial(Material material)
    {
        if (headRenderer == null || material == null) return;

        Material[] mats = headRenderer.materials;

        if (faceMaterialIndex < 0 || faceMaterialIndex >= mats.Length) return;

        mats[faceMaterialIndex] = material;
        headRenderer.materials = mats;
    }
}