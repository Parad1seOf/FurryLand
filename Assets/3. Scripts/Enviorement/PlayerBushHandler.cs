using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBushHandler : MonoBehaviour, ISpeedModifier
{
    [Header("Bush Settings")]
    [SerializeField] private LayerMask bushLayer;
    [SerializeField] private float speedReduction = 5f;

    [Header("Post Process")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float transitionSpeed = 5f;

    private PlayerController playerController;

    private int bushesInside = 0;
    private Coroutine volumeCoroutine;

    private bool speedModifierApplied = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (postProcessVolume != null)
        {
            postProcessVolume.weight = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsBush(other.gameObject)) return;

        bushesInside++;

        if (!speedModifierApplied)
        {
            playerController.AddSpeedModifier(this);
            speedModifierApplied = true;
        }

        if (bushesInside == 1)
        {
            StartVolumeTransition(1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsBush(other.gameObject)) return;

        bushesInside--;

        if (bushesInside <= 0)
        {
            bushesInside = 0;

            if (speedModifierApplied)
            {
                playerController.RemoveSpeedModifier(this);
                speedModifierApplied = false;
            }

            StartVolumeTransition(0f);
        }
    }

    private bool IsBush(GameObject obj)
    {
        return (bushLayer.value & (1 << obj.layer)) != 0;
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

    private IEnumerator LerpVolume(float target)
    {
        while (!Mathf.Approximately(postProcessVolume.weight, target))
        {
            postProcessVolume.weight = Mathf.Lerp(
                postProcessVolume.weight,
                target,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }

        postProcessVolume.weight = target;
    }

    public float GetValue()
    {
        return -speedReduction;
    }
}