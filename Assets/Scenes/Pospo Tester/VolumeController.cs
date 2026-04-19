using UnityEngine;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviour
{
    [Header("Referencia")]
    public Volume volume;

    [Header("Configuración")]
    public float currentValue = 0f; // 0 a 1
    public float increaseAmount = 0.1f;
    public float decreaseAmountPerFrame = 0.01f;
    public float delayBeforeDecrease = 3f;

    private float timer = 0f;
    private bool isDecreasing = false;

    void Update()
    {
        // Pulsar X → subir
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentValue += increaseAmount;

            // Clamp para no pasar de 1
            currentValue = Mathf.Clamp01(currentValue);

            // Reiniciar temporizador
            timer = 0f;
            isDecreasing = false;

            UpdateVolume();
        }

        // Contador de tiempo
        timer += Time.deltaTime;

        // Activar bajada tras el delay
        if (!isDecreasing && timer >= delayBeforeDecrease)
        {
            isDecreasing = true;
        }

        // Bajada progresiva por frame
        if (isDecreasing && currentValue > 0f)
        {
            currentValue -= decreaseAmountPerFrame;

            // Evitar negativos
            currentValue = Mathf.Max(currentValue, 0f);

            UpdateVolume();
        }
    }

    void UpdateVolume()
    {
        volume.weight = currentValue;
    }
}