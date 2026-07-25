using UnityEngine;
using VLB;

/// <summary>
/// Handles individual noise-based flickering for a single light and optional VolumetricLightBeamSD.
/// </summary>
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("How much the intensity can drop (0 = no flicker, 1 = can go fully off)")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerAmount = 0.85f;

    [Tooltip("How fast the flicker changes (higher = more erratic)")]
    [SerializeField] private float flickerSpeed = 25f;

    [Header("Volumetric Beam Settings")]
    [Tooltip("Base center value for VolumetricLightBeamSD.intensityMultiplier")]
    [SerializeField] private float baseBeamMultiplier = 0.12f;

    [Header("Occasional Dropout")]
    [Tooltip("Chance per second of a brief full dropout")]
    [SerializeField] private float dropoutChancePerSecond = 0.35f;
    [SerializeField] private float dropoutDuration = 0.15f;

    [Header("References")]
    [SerializeField] private Light targetLight;
    [SerializeField] private VolumetricLightBeamSD targetBeam;

    private float _baseLightIntensity;
    private float _dropoutTimer;
    private float _noiseOffset;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetBeam == null)
            targetBeam = GetComponent<VolumetricLightBeamSD>();

        if (targetLight != null)
            _baseLightIntensity = targetLight.intensity;

        _noiseOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        float currentMult = 1f;

        // Check for dropout
        if (_dropoutTimer > 0f)
        {
            _dropoutTimer -= Time.deltaTime;
            currentMult = 0.05f;
        }
        else if (Random.value < dropoutChancePerSecond * Time.deltaTime)
        {
            _dropoutTimer = dropoutDuration;
            currentMult = 0.05f;
        }
        else
        {
            // Perlin noise flicker with random offset per instance
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + _noiseOffset, 0f);
            currentMult = 1f - (noise * flickerAmount);
        }

        // Apply to light
        if (targetLight != null)
        {
            targetLight.intensity = _baseLightIntensity * currentMult;
        }

        // Apply ONLY to intensityMultiplier centered around baseBeamMultiplier (0.12)
        if (targetBeam != null)
        {
            targetBeam.intensityMultiplier = baseBeamMultiplier * currentMult;
        }
    }
}
