using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Bush : MonoBehaviour, ISpeedModifier
{
    [Header("Speed")]
    [SerializeField] private float speedReduction = 5f;

    [Header("Post Process")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float transitionSpeed = 2f;

    private Coroutine volumeCoroutine;

    private void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.weight = 0f;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.AddSpeedModifier(this);

        StartVolumeTransition(1f);
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.RemoveSpeedModifier(this);

        StartVolumeTransition(0f);
    }

    private void StartVolumeTransition(float target)
    {
        if (postProcessVolume == null) return;

        if (volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }

        volumeCoroutine = StartCoroutine(LerpVolume(target));
    }

    private IEnumerator LerpVolume(float targetWeight)
    {
        while (!Mathf.Approximately(postProcessVolume.weight, targetWeight))
        {
            postProcessVolume.weight = Mathf.Lerp(
                postProcessVolume.weight,
                targetWeight,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }

        postProcessVolume.weight = targetWeight;
    }

    public float GetValue()
    {
        return -speedReduction;
    }
}