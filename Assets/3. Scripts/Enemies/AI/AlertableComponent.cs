using System;
using UnityEngine;

public class AlertableComponent : MonoBehaviour, IAlertable
{
    [SerializeField] private EnemyStateMachine stateMachine;

    public void Start()
    {
        if (stateMachine == null) stateMachine = GetComponent<EnemyStateMachine>();
        AlertSystem.Instance.OnAlertTriggered += GetAlerted;
    }

    public void GetAlerted()
    {
        stateMachine.Alert();
    }
}
