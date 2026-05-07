using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    public abstract bool MeetsCondition(PlayerController player);

    //Esto asi no
    public abstract void FulfillCondition(PlayerController player);
}
