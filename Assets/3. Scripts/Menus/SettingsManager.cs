using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Toggle fogToggle;

    [SerializeField] private Camera mainCamera;

    private const float DEFAULT_VOLUME = 0.5f;
    private const float DEFAULT_FOV = 60f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_VOLUME);
        float savedFOV = PlayerPrefs.GetFloat("CameraFOV", DEFAULT_FOV);
        int savedFog = PlayerPrefs.GetInt("FogEnabled", 1); // 1 = Activado, 0 = Desactivado

        if (volumeSlider != null) volumeSlider.value = savedVolume;
        if (FOVSlider != null) FOVSlider.value = savedFOV;
        if (fogToggle != null) fogToggle.isOn = savedFog == 1;

        UpdateVolume(savedVolume);
        UpdateFOV(savedFOV);
        UpdateFog(savedFog == 1);

        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(UpdateVolume);
        if (FOVSlider != null) FOVSlider.onValueChanged.AddListener(UpdateFOV);
        if (fogToggle != null) fogToggle.onValueChanged.AddListener(UpdateFog);
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
}