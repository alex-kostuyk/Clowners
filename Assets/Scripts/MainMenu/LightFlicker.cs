using UnityEngine;

/// <summary>
/// Randomly flickers a light's intensity to simulate unstable/old fluorescent lighting.
/// Attach to any GameObject with a Light component, or assign lights manually.
/// </summary>
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("How much the intensity can drop (0 = no flicker, 1 = can go fully off)")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerAmount = 0.15f;

    [Tooltip("How fast the flicker changes (higher = more erratic)")]
    [SerializeField] private float flickerSpeed = 8f;

    [Header("Occasional Dropout")]
    [Tooltip("Chance per second of a brief full dropout")]
    [SerializeField] private float dropoutChancePerSecond = 0.05f;
    [SerializeField] private float dropoutDuration = 0.08f;

    [Header("References")]
    [SerializeField] private Light[] lights;

    private float[] _baseIntensities;
    private float _dropoutTimer;
    private float _noiseOffset;

    private void Awake()
    {
        if (lights == null || lights.Length == 0)
        {
            var l = GetComponent<Light>();
            if (l != null) lights = new Light[] { l };
        }

        if (lights != null && lights.Length > 0)
        {
            _baseIntensities = new float[lights.Length];
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                    _baseIntensities[i] = lights[i].intensity;
            }
        }

        _noiseOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (lights == null) return;

        // Check for dropout
        if (_dropoutTimer > 0f)
        {
            _dropoutTimer -= Time.deltaTime;
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                    lights[i].intensity = _baseIntensities[i] * 0.05f;
            }
            return;
        }

        // Random dropout trigger
        if (Random.value < dropoutChancePerSecond * Time.deltaTime)
        {
            _dropoutTimer = dropoutDuration;
            return;
        }

        // Perlin noise flicker
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + _noiseOffset, 0f);
        float flickerMult = 1f - (noise * flickerAmount);

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].intensity = _baseIntensities[i] * flickerMult;
        }
    }
}
