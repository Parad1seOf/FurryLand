// Destruye los bullet holes spawneados por GunSystem tras un tiempo determinado..
// para evitar la acumulación de decals en escena y opimizar
using System.Collections;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{
    public float lifeTime = 1.0f;     
    public float maxLifeTime = 10.0f;  

    private IEnumerator Start()
    {
        float randomTime = Random.Range(lifeTime, maxLifeTime);
        yield return new WaitForSeconds(randomTime);
        Destroy(gameObject);
    }
}