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
    [SerializeField] private Slider sensitivityXSlider;
    [SerializeField] private Slider sensitivityYSlider;
    [SerializeField] private TextMeshProUGUI sensitivityXLabel;
    [SerializeField] private TextMeshProUGUI sensitivityYLabel;

    [Header("Post Processing")]
    [SerializeField] private Toggle postProcessingToggle;

    private ExampleCharacterCamera _camera;
    private PostProcessVolume _postProcessVolume;

    public System.Action OnBack;

    private void Awake()
    {
        _camera = FindObjectOfType<ExampleCharacterCamera>();
        _postProcessVolume = FindObjectOfType<PostProcessVolume>();

        // Volume slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeLabel();
        }

        // Sensitivity X slider
        if (sensitivityXSlider != null)
        {
            sensitivityXSlider.minValue = 0.1f;
            sensitivityXSlider.maxValue = 5f;
            sensitivityXSlider.value = _camera != null ? _camera.RotationSpeedX : 1f;
            sensitivityXSlider.onValueChanged.AddListener(OnSensitivityXChanged);
            UpdateSensitivityXLabel();
        }

        // Sensitivity Y slider
        if (sensitivityYSlider != null)
        {
            sensitivityYSlider.minValue = 0.1f;
            sensitivityYSlider.maxValue = 5f;
            sensitivityYSlider.value = _camera != null ? _camera.RotationSpeedY : 1f;
            sensitivityYSlider.onValueChanged.AddListener(OnSensitivityYChanged);
            UpdateSensitivityYLabel();
        }

        // Post-processing toggle
        if (postProcessingToggle != null)
        {
            postProcessingToggle.isOn = _postProcessVolume != null && _postProcessVolume.enabled;
            postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggled);
        }

        // Back button
        if (backButton != null)
            backButton.onClick.AddListener(Hide);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
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

    private void OnSensitivityXChanged(float value)
    {
        if (_camera != null) _camera.RotationSpeedX = value;
        UpdateSensitivityXLabel();
    }

    private void OnSensitivityYChanged(float value)
    {
        if (_camera != null) _camera.RotationSpeedY = value;
        UpdateSensitivityYLabel();
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

    private void UpdateSensitivityXLabel()
    {
        if (sensitivityXLabel != null)
            sensitivityXLabel.text = $"Sensitivity X: {sensitivityXSlider.value:F1}";
    }

    private void UpdateSensitivityYLabel()
    {
        if (sensitivityYLabel != null)
            sensitivityYLabel.text = $"Sensitivity Y: {sensitivityYSlider.value:F1}";
    }
}
