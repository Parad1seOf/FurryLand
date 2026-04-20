public class AIContext
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public AIMovementComponent movement;
    public ITarget target;

    public AIContext(IChangeState changeState ,DetectionComponent detection, AIMovementComponent movement)
    {
        this.changeState = changeState;
        this.detection = detection;
        this.movement = movement;
    }
}
