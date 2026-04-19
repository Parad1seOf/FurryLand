using UnityEngine;
using System;
using System.Collections.Generic;

public class AlertSystem : MonoBehaviour
{
    public static AlertSystem Instance { get; private set; }

    public event Action OnAlertTriggered;

    private readonly List<SuspicionComponent> registry = new List<SuspicionComponent>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(SuspicionComponent sus)
    {
        if (!registry.Contains(sus)) registry.Add(sus);
    }

    public void Unregister(SuspicionComponent sus) => registry.Remove(sus);

    public IReadOnlyList<SuspicionComponent> GetAllSuspicions() => registry;

    public void TriggerAlert() => OnAlertTriggered?.Invoke();
}