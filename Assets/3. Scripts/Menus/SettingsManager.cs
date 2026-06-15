using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private FirstPersonLook firstPersonLook;

    [Header("UI references")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Toggle fogToggle;
    [SerializeField] private Slider sensitivitySlider;

    [SerializeField] private Camera mainCamera;

    private const float DEFAULT_VOLUME = 0.5f;
    private const float DEFAULT_FOV = 60f;
    private const float DEFAULT_SENSITIVITY = 120f;


    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_VOLUME);
        float savedFOV = PlayerPrefs.GetFloat("CameraFOV", DEFAULT_FOV);
        int savedFog = PlayerPrefs.GetInt("FogEnabled", 1); // 1 = Activado, 0 = Desactivado
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", DEFAULT_SENSITIVITY);

        if (volumeSlider != null) volumeSlider.value = savedVolume;
        if (FOVSlider != null) FOVSlider.value = savedFOV;
        if (fogToggle != null) fogToggle.isOn = savedFog == 1;
        if (sensitivitySlider != null) sensitivitySlider.value = savedSensitivity;
        if (firstPersonLook == null) firstPersonLook = FindFirstObjectByType<FirstPersonLook>();

        UpdateVolume(savedVolume);
        UpdateFOV(savedFOV);
        UpdateFog(savedFog == 1);
        UpdateSensitivity(savedSensitivity);

        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(UpdateVolume);
        if (FOVSlider != null) FOVSlider.onValueChanged.AddListener(UpdateFOV);
        if (fogToggle != null) fogToggle.onValueChanged.AddListener(UpdateFog);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    public void UpdateVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void UpdateFOV(float value)
    {
        if (mainCamera != null)
            mainCamera.fieldOfView = value;

        PlayerPrefs.SetFloat("CameraFOV", value);
    }

    public void UpdateFog(bool value)
    {
        RenderSettings.fog = value;
        PlayerPrefs.SetInt("FogEnabled", value ? 1 : 0);
    }

    public void UpdateSensitivity(float value)
    {
        if (firstPersonLook != null)
        {
            firstPersonLook.yawSpeed = value;
            firstPersonLook.pitchSpeed = value;
        }

        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }
}