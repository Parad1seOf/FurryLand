using System;
using UnityEngine;

public class AlertableComponent : MonoBehaviour, IAlertable
{
    [SerializeField] private EnemyStateMachine stateMachine;

    void Awake()
    {
        if (stateMachine == null) stateMachine = GetComponent<EnemyStateMachine>();
        
    }

    public void Start()
    {
        AlertSystem.Instance.OnAlertTriggered += GetAlerted;
    }

    public void GetAlerted()
    {
        stateMachine.Alert();
    }
}
