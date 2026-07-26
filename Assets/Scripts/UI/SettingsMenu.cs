using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using KinematicCharacterController.Examples;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backButton;

    [Header("Sound")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeLabel;

    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityLabel;

    [Header("Post Processing")]
    [SerializeField] private Toggle postProcessingToggle;

    private ExampleCharacterCamera _camera;
    private PostProcessVolume _postProcessVolume;

    public System.Action OnBack;

    // PlayerPrefs Keys
    private const string VolumeKey = "Settings_Volume";
    private const string SensitivityKey = "Settings_Sensitivity";
    private const string PostProcessingKey = "Settings_PostProcessing";

    private void Awake()
    {
        InitializeSettings();
        SetupListeners();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        InitializeSettings();
        SetupListeners();
    }

    private void InitializeSettings()
    {
        if (_camera == null)
            _camera = FindObjectOfType<ExampleCharacterCamera>();
        if (_postProcessVolume == null)
            _postProcessVolume = FindObjectOfType<PostProcessVolume>();

        // Load saved values or fall back to defaults
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
        bool savedPostProcessing = PlayerPrefs.GetInt(PostProcessingKey, 1) == 1;

        // Apply loaded values to game systems
        AudioListener.volume = savedVolume;

        if (_camera != null)
        {
            _camera.RotationSpeedX = savedSensitivity;
            _camera.RotationSpeedY = savedSensitivity;
        }

        if (_postProcessVolume != null)
        {
            _postProcessVolume.enabled = savedPostProcessing;
        }

        // Apply loaded values to UI
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = savedVolume;
            UpdateVolumeLabel();
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = savedSensitivity;
            UpdateSensitivityLabel();
        }

        if (postProcessingToggle != null)
        {
            postProcessingToggle.isOn = savedPostProcessing;
        }
    }

    private void SetupListeners()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (postProcessingToggle != null)
        {
            postProcessingToggle.onValueChanged.RemoveListener(OnPostProcessingToggled);
            postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggled);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(Hide);
            backButton.onClick.AddListener(Hide);
        }
    }

    public void Show()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void Hide()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        OnBack?.Invoke();
    }

    public bool IsVisible => settingsPanel != null && settingsPanel.activeSelf;

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
        UpdateVolumeLabel();
    }

    private void OnSensitivityChanged(float value)
    {
        if (_camera != null)
        {
            _camera.RotationSpeedX = value;
            _camera.RotationSpeedY = value;
        }

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
        UpdateSensitivityLabel();
    }

    private void OnPostProcessingToggled(bool isOn)
    {
        if (_postProcessVolume != null)
            _postProcessVolume.enabled = isOn;

        PlayerPrefs.SetInt(PostProcessingKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateVolumeLabel()
    {
        if (volumeLabel != null)
            volumeLabel.text = $"Volume: {Mathf.RoundToInt(AudioListener.volume * 100)}%";
    }

    private void UpdateSensitivityLabel()
    {
        if (sensitivityLabel != null && sensitivitySlider != null)
            sensitivityLabel.text = $"Sensitivity: {sensitivitySlider.value:F1}";
    }
}