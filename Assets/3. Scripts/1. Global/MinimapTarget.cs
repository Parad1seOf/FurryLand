using System.Collections.Generic;
using UnityEngine;

public class MinimapTarget : MonoBehaviour
{
    [Header("Apariencia en el minimapa")]
    public Color color = Color.red;
    [Range(2f, 40f)] public float size = 10f;
    [Tooltip("Opcional. Si es null se usa el sprite por defecto del Minimap.")]
    public Sprite customSprite;

    private static readonly HashSet<MinimapTarget> all = new();
    public static IEnumerable<MinimapTarget> All => all;

    private void OnEnable()  => all.Add(this);
    private void OnDisable() => all.Remove(this);
}