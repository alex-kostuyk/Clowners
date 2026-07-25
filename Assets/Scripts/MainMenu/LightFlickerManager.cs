using UnityEngine;
using VLB;

/// <summary>
/// Central manager that controls flicker parameters for all assigned lights/beams from one place,
/// while generating unique, independent random noise and dropouts per light.
/// </summary>
public class LightFlickerManager : MonoBehaviour
{
    [System.Serializable]
    public class FlickeringLightPair
    {
        public string name;
        public Light light;
        public VolumetricLightBeamSD beam;

        [HideInInspector] public float baseLightIntensity;
        [HideInInspector] public float noiseOffset;
        [HideInInspector] public float dropoutTimer;
        [HideInInspector] public float speedMultiplier;
    }

    [Header("Global Flicker Settings")]
    [Tooltip("How much the intensity can drop (0 = no flicker, 1 = can go fully off)")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerAmount = 0.85f;

    [Tooltip("How fast the flicker changes (higher = more erratic)")]
    [SerializeField] private float flickerSpeed = 22f;

    [Header("Volumetric Beam Settings")]
    [Tooltip("Base center value for VolumetricLightBeamSD.intensityMultiplier")]
    [SerializeField] private float baseBeamMultiplier = 0.12f;

    [Header("Occasional Dropout")]
    [Tooltip("Chance per second for an individual light to drop out")]
    [SerializeField] private float dropoutChancePerSecond = 0.25f;
    [SerializeField] private float dropoutDuration = 0.12f;

    [Header("Light Pairs")]
    [SerializeField] private FlickeringLightPair[] pairs;

    private void Awake()
    {
        if (pairs == null) return;

        for (int i = 0; i < pairs.Length; i++)
        {
            var p = pairs[i];
            if (p == null) continue;

            if (p.light != null)
                p.baseLightIntensity = p.light.intensity;

            // Unique random offset and slight speed variation per light
            p.noiseOffset = Random.Range(0f, 2000f);
            p.speedMultiplier = Random.Range(0.85f, 1.15f);
        }
    }

    private void Update()
    {
        if (pairs == null) return;

        for (int i = 0; i < pairs.Length; i++)
        {
            var p = pairs[i];
            if (p == null) continue;

            float currentMult = 1f;

            // Check for independent dropout per light
            if (p.dropoutTimer > 0f)
            {
                p.dropoutTimer -= Time.deltaTime;
                currentMult = 0.05f;
            }
            else if (Random.value < dropoutChancePerSecond * Time.deltaTime)
            {
                p.dropoutTimer = dropoutDuration;
                currentMult = 0.05f;
            }
            else
            {
                // Independent Perlin noise calculation per light
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed * p.speedMultiplier + p.noiseOffset, 0f);
                currentMult = 1f - (noise * flickerAmount);
            }

            // Apply independent values to this light
            if (p.light != null)
            {
                p.light.intensity = p.baseLightIntensity * currentMult;
            }

            // Apply independent values to this volumetric beam multiplier
            if (p.beam != null)
            {
                p.beam.intensityMultiplier = baseBeamMultiplier * currentMult;
            }
        }
    }
}
