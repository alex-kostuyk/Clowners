using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlitchPulse : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float minTimeBetweenPulses = 3f;
    [SerializeField] private float maxTimeBetweenPulses = 8f;
    [SerializeField] private float pulseDuration = 2f;
    [SerializeField] private float fadeInSpeed = 6f;
    [SerializeField] private float fadeOutSpeed = 3f;

    [Header("Intensity")]
    [SerializeField] private float baseVignetteIntensity = 0.4f;
    [SerializeField] private float pulseVignetteIntensity = 0.75f;
    [SerializeField] private float baseChromaticAberration = 0.15f;
    [SerializeField] private float pulseChromaticAberration = 1.2f;

    [Header("References")]
    [SerializeField] private PostProcessVolume postProcessVolume;

    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;

    private float _nextPulseTime;
    private float _pulseEndTime;
    private bool _isPulsing;
    private float _currentIntensity; // 0 = baseline, 1 = full pulse

    private void Start()
    {
        if (postProcessVolume == null)
            postProcessVolume = FindObjectOfType<PostProcessVolume>();

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGetSettings(out _vignette);
            postProcessVolume.profile.TryGetSettings(out _chromaticAberration);
        }

        ScheduleNextPulse();
        ApplyIntensity(0f);
    }

    private void Update()
    {
        if (Time.time >= _nextPulseTime && !_isPulsing)
        {
            _isPulsing = true;
            _pulseEndTime = Time.time + pulseDuration;
        }

        if (_isPulsing)
        {
            if (Time.time < _pulseEndTime)
            {
                _currentIntensity = Mathf.MoveTowards(_currentIntensity, 1f, fadeInSpeed * Time.deltaTime);
            }
            else
            {
                _currentIntensity = Mathf.MoveTowards(_currentIntensity, 0f, fadeOutSpeed * Time.deltaTime);
                if (_currentIntensity <= 0f)
                {
                    _isPulsing = false;
                    ScheduleNextPulse();
                }
            }
        }

        ApplyIntensity(_currentIntensity);
    }

    private void ApplyIntensity(float t)
    {
        if (_vignette != null)
        {
            _vignette.intensity.value = Mathf.Lerp(baseVignetteIntensity, pulseVignetteIntensity, t);
        }
        if (_chromaticAberration != null)
        {
            _chromaticAberration.intensity.value = Mathf.Lerp(baseChromaticAberration, pulseChromaticAberration, t);
        }
    }

    private void ScheduleNextPulse()
    {
        _nextPulseTime = Time.time + Random.Range(minTimeBetweenPulses, maxTimeBetweenPulses);
    }
}
