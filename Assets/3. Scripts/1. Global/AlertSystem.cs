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
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
            Debug.Log("Adios");
            return; 
        }
        Debug.Log(gameObject.name);
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void Register(IAlertable alertable)
    {
        if (!registry.Contains(alertable)) registry.Add(alertable);
    }

    public void Unregister(IAlertable alertable) => registry.Remove(alertable);

    public IReadOnlyList<IAlertable> GetAllSuspicions() => registry;

    public void TriggerAlert()
    {
        if (IsAlreadyTriggered) return;
        IsAlreadyTriggered = true;
        OnAlertTriggered?.Invoke();
    }
}