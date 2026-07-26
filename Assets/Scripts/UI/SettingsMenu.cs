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

        // Volume slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            UpdateVolumeLabel();
        }

        // Sensitivity slider
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = _camera != null ? _camera.RotationSpeedX : 1f;
            UpdateSensitivityLabel();
        }

        // Post-processing toggle
        if (postProcessingToggle != null)
        {
            postProcessingToggle.isOn = _postProcessVolume != null && _postProcessVolume.enabled;
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

    private void Update()
    {
        SetupListeners();
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
        UpdateVolumeLabel();
    }

    private void OnSensitivityChanged(float value)
    {
        if (_camera != null)
        {
            _camera.RotationSpeedX = value;
            _camera.RotationSpeedY = value;
        }
        UpdateSensitivityLabel();
    }

    private void OnPostProcessingToggled(bool isOn)
    {
        if (_postProcessVolume != null) _postProcessVolume.enabled = isOn;
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
