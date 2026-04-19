public class AIContext
{
    public IChangeState changeState;
    public DetectionComponent detection;
    public ITarget target;

    public AIContext(IChangeState changeState ,DetectionComponent detection)
    {
        this.changeState = changeState;
        this.detection = detection;
    }
}
