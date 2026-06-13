using UnityEngine;

public class AIContext : MonoBehaviour
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public AIMovementComponent movement;
    public ITarget target;
    public EnemyAttack attack;
    public EnemyDisplay display;
    public EnemyAwarenessComponent awareness;
    public IAIBehaviour behaviour;
    public AIPathingComponent pathing;
    public WaterGun waterGun;
    public IdleMovements idle;
    public RabbitFaceExpression faceExpression;


    void Awake()
    {
        if (detection == null)
            detection = GetComponent<DetectionComponent>();
        if (movement == null)
            movement = GetComponent<AIMovementComponent>();
        if (attack == null)
            attack = GetComponent<EnemyAttack>();
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
        if (waterGun == null)
            waterGun = GetComponent<WaterGun>();
        if (idle == null)
            idle = GetComponent<IdleMovements>();
        if (faceExpression == null)
            faceExpression = GetComponent<RabbitFaceExpression>();

    }
}
