using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class FirstPersonLook : MonoBehaviour
{
    [Header("References")]
    public Transform pitchController;

    [Header("Settings")]
    public float yawSpeed   = 120f;
    public float pitchSpeed = 120f;
    public float minPitch   = -80f;
    public float maxPitch   =  80f;

    public float Yaw   { get; private set; }
    public float Pitch { get; private set; }

    private InputReader input;

    private void Awake()
    {
        input = GetComponent<InputReader>();
        Yaw   = transform.eulerAngles.y;
        Pitch = pitchController != null ? pitchController.localEulerAngles.x : 0f;
    }

    private void Update()
    {
        Yaw   += input.LookInput.x * yawSpeed   * Time.deltaTime;
        Pitch -= input.LookInput.y * pitchSpeed * Time.deltaTime;
        Pitch  = Mathf.Clamp(Pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, Yaw, 0f);
        if (pitchController != null)
            pitchController.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
    }

    public void ShakeCamera(float duration, AnimationCurve strength)
    {
        StartCoroutine(Shake(duration, strength));
    }

    public IEnumerator Shake(float duration, AnimationCurve strength)
    {
        Transform cam = Camera.main.transform;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float currentStrength = strength.Evaluate(timer / duration);
            cam.position = pitchController.position + Random.insideUnitSphere * currentStrength;
            yield return null;
        }

        cam.position = pitchController.position;
    }
}