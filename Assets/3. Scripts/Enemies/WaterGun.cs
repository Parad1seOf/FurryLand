using UnityEngine;

public class WaterGun : MonoBehaviour
{
    public float damagePerSecond = 2f;
    public Explosives target;

    public void Water()
    {
        target.GetWatered(damagePerSecond);
    }
}
