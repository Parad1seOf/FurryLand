using UnityEngine;

public class AIContext : MonoBehaviour
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public AIMovementComponent movement;
    public ITarget target;
    public IAttack attack;
    public EnemyDisplay display;
    public EnemyAwarenessComponent awareness;
    public IAIBehaviour behaviour;
    public AIPathingComponent pathing;


    void Start()
    {
        if (detection == null)
            detection = GetComponent<DetectionComponent>();
        if (movement == null)
            movement = GetComponent<AIMovementComponent>();
        if (attack == null)
            attack = GetComponent<IAttack>();
        if (display == null)
            display = GetComponent<EnemyDisplay>();
        if (awareness == null)
            awareness = GetComponent<EnemyAwarenessComponent>();
        if (changeState == null)
            changeState = GetComponent<IChangeState>();
        if (behaviour == null)
            behaviour = GetComponent<IAIBehaviour>();
        if (pathing == null)
            pathing = GetComponent<AIPathingComponent>();

    }
}
