using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteMoodVisualizer : MonoBehaviour
{
    [SerializeField]
    private MeteoriteEye _meteoriteEye;
    [SerializeField]
    private MeteoriteBlobAnimator _meteoriteBlobAnimator;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Light _pointLight;

    [Header("Shader Property Names")]
    [SerializeField]
    private string _colorPropertyName = "_Color";
    [SerializeField]
    private string _emissionColorPropertyName = "_EmissionColor";

    [Header("Visual Curve & Smoothing")]
    [SerializeField]
    private AnimationCurve _stabilityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    private float _smoothSpeed = 5f;

    [Header("Reaction Settings")]
    [SerializeField]
    private float _reactionDuration = 2f;
    [SerializeField, Range(0f, 1f)]
    private float _wrongAnswerInstabilityPercent = 0.25f;

    private Coroutine _reactionRoutine;
    private float _currentStability = 1f;

    // Target values for smooth interpolation
    private float _targetBlobMaxOffset;
    private float _targetBlobSpeed;
    private float _targetEyePauseDuration;
    private float _targetEyeJitterAngle;

    // Color targets for smooth interpolation
    private Color _targetColor = Color.white;
    private Color _currentColor = Color.white;

    private void Start()
    {
        ApplyStabilityVisuals(_currentStability);
        SnapVisualsToTarget();
    }

    private void OnEnable()
    {
        MeteoriteEntity.OnStabilityChange += HandleStabilityChange;
        MeteoriteEntity.OnRequestChange += HandleRequestChange;
    }

    private void OnDisable()
    {
        MeteoriteEntity.OnStabilityChange -= HandleStabilityChange;
        MeteoriteEntity.OnRequestChange -= HandleRequestChange;

        if (_reactionRoutine != null)
        {
            StopCoroutine(_reactionRoutine);
            _reactionRoutine = null;
        }
    }

    private void Update()
    {
        SmoothUpdateVisuals();
    }

    private void HandleStabilityChange(float stability)
    {
        _currentStability = Mathf.Clamp01(stability);

        if (_reactionRoutine == null)
        {
            ApplyStabilityVisuals(_currentStability);
        }
    }

    private void HandleRequestChange(RequestProperty requestProperty)
    {
        if (requestProperty == null) return;
        _targetColor = requestProperty.RequestColor;
    }

    private void ApplyStabilityVisuals(float stability)
    {
        float rawInstability = 1f - stability;
        float t = _stabilityCurve != null ? _stabilityCurve.Evaluate(rawInstability) : rawInstability;

        _targetBlobMaxOffset = Mathf.Lerp(0.0002f, 0.005f, t);
        _targetBlobSpeed = Mathf.Lerp(2f, 50f, t);

        _targetEyePauseDuration = Mathf.Lerp(0.3f, 0.003f, t);
        _targetEyeJitterAngle = Mathf.Lerp(2f, 19f, t);
    }

    private void SmoothUpdateVisuals()
    {
        float lerpDelta = Time.deltaTime * _smoothSpeed;

        // Smoothly interpolate parameters
        if (_meteoriteBlobAnimator != null)
        {
            _meteoriteBlobAnimator.MaxOffset = Mathf.Lerp(_meteoriteBlobAnimator.MaxOffset, _targetBlobMaxOffset, lerpDelta);
            _meteoriteBlobAnimator.Speed = Mathf.Lerp(_meteoriteBlobAnimator.Speed, _targetBlobSpeed, lerpDelta);
        }

        if (_meteoriteEye != null)
        {
            _meteoriteEye._pauseDuration = Mathf.Lerp(_meteoriteEye._pauseDuration, _targetEyePauseDuration, lerpDelta);
            _meteoriteEye._jitterAngle = Mathf.Lerp(_meteoriteEye._jitterAngle, _targetEyeJitterAngle, lerpDelta);
        }

        // Smoothly interpolate material and light colors
        _currentColor = Color.Lerp(_currentColor, _targetColor, lerpDelta);

        if (_pointLight != null)
        {
            _pointLight.color = _currentColor;
        }

        if (_material != null)
        {
            _material.SetColor(_colorPropertyName, _currentColor);
            _material.SetColor(_emissionColorPropertyName, _currentColor);
        }
    }

    private void SnapVisualsToTarget()
    {
        if (_meteoriteBlobAnimator != null)
        {
            _meteoriteBlobAnimator.MaxOffset = _targetBlobMaxOffset;
            _meteoriteBlobAnimator.Speed = _targetBlobSpeed;
        }

        if (_meteoriteEye != null)
        {
            _meteoriteEye._pauseDuration = _targetEyePauseDuration;
            _meteoriteEye._jitterAngle = _targetEyeJitterAngle;
        }

        _currentColor = _targetColor;
        if (_pointLight != null) _pointLight.color = _currentColor;
        if (_material != null)
        {
            _material.SetColor(_colorPropertyName, _currentColor);
            _material.SetColor(_emissionColorPropertyName, _currentColor);
        }
    }

    public void OnRightAnswerReaction()
    {
        StartReactionOverride(
            blobMaxOffset: 0.0002f,
            blobSpeed: 2f,
            eyePauseDuration: 0.01f,
            eyeJitterAngle: 0.5f
        );
    }

    public void OnWrongAnswerReaction()
    {
        float currentInstability = 1f - _currentStability;
        float boostedInstability = Mathf.Clamp01(currentInstability + _wrongAnswerInstabilityPercent);
        float t = _stabilityCurve != null ? _stabilityCurve.Evaluate(boostedInstability) : boostedInstability;

        float calculatedBlobMaxOffset = Mathf.Max(0.0004f, Mathf.Lerp(0.0002f, 0.005f, t));
        float calculatedBlobSpeed = Mathf.Max(15f, Mathf.Lerp(2f, 50f, t));
        float calculatedEyePauseDuration = Mathf.Min(0.01f, Mathf.Lerp(0.3f, 0.003f, t));
        float calculatedEyeJitterAngle = Mathf.Max(0.5f, Mathf.Lerp(2f, 19f, t));

        StartReactionOverride(
            blobMaxOffset: calculatedBlobMaxOffset,
            blobSpeed: calculatedBlobSpeed,
            eyePauseDuration: calculatedEyePauseDuration,
            eyeJitterAngle: calculatedEyeJitterAngle
        );
    }

    private void StartReactionOverride(float blobMaxOffset, float blobSpeed, float eyePauseDuration, float eyeJitterAngle)
    {
        if (_reactionRoutine != null)
        {
            StopCoroutine(_reactionRoutine);
        }

        _reactionRoutine = StartCoroutine(ReactionOverrideRoutine(blobMaxOffset, blobSpeed, eyePauseDuration, eyeJitterAngle));
    }

    private IEnumerator ReactionOverrideRoutine(float blobMaxOffset, float blobSpeed, float eyePauseDuration, float eyeJitterAngle)
    {
        _targetBlobMaxOffset = blobMaxOffset;
        _targetBlobSpeed = blobSpeed;
        _targetEyePauseDuration = eyePauseDuration;
        _targetEyeJitterAngle = eyeJitterAngle;

        yield return new WaitForSeconds(_reactionDuration);

        _reactionRoutine = null;
        ApplyStabilityVisuals(_currentStability);
    }
}