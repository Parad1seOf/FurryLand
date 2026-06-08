using UnityEngine;

public class ShadowOnly : MonoBehaviour
{
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }
}