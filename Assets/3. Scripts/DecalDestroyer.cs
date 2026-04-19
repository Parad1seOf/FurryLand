// Destruye los bullet holes spawneados por GunSystem tras un tiempo determinado..
// para evitar la acumulación de decals en escena y opimizar
using System.Collections;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{
    public float lifeTime = 5.0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}