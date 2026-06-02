using UnityEngine;
using System;
using System.Collections.Generic;

public class AlertSystem : MonoBehaviour
{
    public static AlertSystem Instance { get; private set; }

    public event Action OnAlertTriggered;

    public bool IsAlreadyTriggered;

    private readonly List<IAlertable> registry = new List<IAlertable>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(IAlertable alertable)
    {
        if (!registry.Contains(alertable)) registry.Add(alertable);
    }

    public void Unregister(IAlertable alertable) => registry.Remove(alertable);

    public IReadOnlyList<IAlertable> GetAllSuspicions() => registry;

    [ContextMenu("TriggerAlert")]
    public void TriggerAlert()
    {
        if (IsAlreadyTriggered) return;
        IsAlreadyTriggered = true;
        OnAlertTriggered?.Invoke();

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopBirdsAmbience();

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Start_Phase2");
    }
}