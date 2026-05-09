public class AIContext
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public AIMovementComponent movement;
    public ITarget target;
    public IAttack attack;
    public EnemyDisplay display;
    public EnemyAwarenessComponent awareness;

    public AIContext(IChangeState changeState ,DetectionComponent detection, 
        AIMovementComponent movement, IAttack attack, EnemyDisplay display,
        EnemyAwarenessComponent awareness)
    {
        this.changeState = changeState;
        this.detection = detection;
        this.movement = movement;
        this.attack = attack;
        this.display = display;
        this.awareness = awareness;
    }
}
