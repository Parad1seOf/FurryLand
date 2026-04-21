public class AIContext
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public AIMovementComponent movement;
    public ITarget target;
    public IAttack attack;

    public AIContext(IChangeState changeState ,DetectionComponent detection, AIMovementComponent movement, IAttack attack)
    {
        this.changeState = changeState;
        this.detection = detection;
        this.movement = movement;
        this.attack = attack;
    }
}
