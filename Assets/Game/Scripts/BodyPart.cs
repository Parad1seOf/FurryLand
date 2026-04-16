// Se adjunta a los colliders hijo de un Enemy. GunSystem lo recibe al impactar
// para aplicar el multiplicador de daño correcto (los padres se van a tener que matar con 2 hits) y disparar el audio adecuado (sistema de headshots? tipo pentakil league of legends?)
using UnityEngine;

public enum BodyPartType { Default, Head, Torso, Limb }

public class BodyPart : MonoBehaviour
{
    [Tooltip("Tipo de parte del cuerpo — se usa para el feedback de audio.")]
    public BodyPartType partType = BodyPartType.Default;

    [Min(0f)]
    public float damageMultiplier = 1f;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GUIStyle style = new GUIStyle
        {
            normal =
            {
                textColor = damageMultiplier >= 1.5f ? Color.red
                          : damageMultiplier  < 1f   ? Color.green
                          : Color.yellow
            }
        };
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.1f,
            $"{partType} ×{damageMultiplier:F2}",
            style
        );
    }
#endif
}