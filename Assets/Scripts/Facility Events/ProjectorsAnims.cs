using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class ProjectorsAnims : MonoBehaviour
{
    public Light projectorsLight;
    public VolumetricLightBeamSD[] volumeCons;
    public float animTotalTime = 10f;
    public float TimeToChangeApearence = 0.15f;
    public float mainLIghtPeakIntensity = 30f;
    public float lightCOnesIntensity = 5f;

    private float _initialLightIntensity;
    private float[] _initialConeIntensities;

    private Coroutine _animRoutine;

    private void Awake()
    {
        CacheInitialValues();
    }

    private void CacheInitialValues()
    {
        if (projectorsLight != null)
        {
            _initialLightIntensity = projectorsLight.intensity;
        }

        if (volumeCons != null && volumeCons.Length > 0)
        {
            _initialConeIntensities = new float[volumeCons.Length];
            for (int i = 0; i < volumeCons.Length; i++)
            {
                if (volumeCons[i] != null)
                {
                    _initialConeIntensities[i] = volumeCons[i].intensityGlobal;
                }
            }
        }
    }

    public void StartBrightLightAnim()
    {
        StartAnimRoutine(mainLIghtPeakIntensity, lightCOnesIntensity);
    }

    public void StartDarkLightAnim()
    {
        StartAnimRoutine(0f, 0f);
    }

    private void StartAnimRoutine(float targetLightIntensity, float targetConeIntensity)
    {
        if (_animRoutine != null)
        {
            StopCoroutine(_animRoutine);
        }

        _animRoutine = StartCoroutine(AnimateLightRoutine(targetLightIntensity, targetConeIntensity));
    }

    private IEnumerator AnimateLightRoutine(float targetLightIntensity, float targetConeIntensity)
    {
        float startLightIntensity = projectorsLight != null ? projectorsLight.intensity : 0f;
        float[] startConeIntensities = GetCurrentConeIntensities();

        // 1. Ramp smoothly to peak/target values over TimeToChangeApearence
        float elapsedTime = 0f;
        while (elapsedTime < TimeToChangeApearence)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / TimeToChangeApearence);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            ApplyIntensities(
                Mathf.Lerp(startLightIntensity, targetLightIntensity, smoothT),
                startConeIntensities,
                targetConeIntensity,
                smoothT
            );

            yield return null;
        }

        // Apply exact target value at peak
        ApplyIntensities(targetLightIntensity, startConeIntensities, targetConeIntensity, 1f);

        // 2. Hold at peak until reaching total duration minus the ramp-down time
        float rampDownDuration = TimeToChangeApearence;
        float holdDuration = Mathf.Max(0f, animTotalTime - TimeToChangeApearence - rampDownDuration);

        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        // 3. Ramp smoothly back to initial states over TimeToChangeApearence
        startLightIntensity = projectorsLight != null ? projectorsLight.intensity : 0f;
        startConeIntensities = GetCurrentConeIntensities();

        elapsedTime = 0f;
        while (elapsedTime < rampDownDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / rampDownDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            ApplyIntensitiesToInitial(startLightIntensity, startConeIntensities, smoothT);

            yield return null;
        }

        // Apply exact initial values at end
        ApplyIntensitiesToInitial(startLightIntensity, startConeIntensities, 1f);
        _animRoutine = null;
    }

    private float[] GetCurrentConeIntensities()
    {
        if (volumeCons == null) return new float[0];
        float[] intensities = new float[volumeCons.Length];
        for (int i = 0; i < volumeCons.Length; i++)
        {
            if (volumeCons[i] != null)
            {
                intensities[i] = volumeCons[i].intensityGlobal;
            }
        }
        return intensities;
    }

    private void ApplyIntensities(float lightVal, float[] startConeVals, float targetConeVal, float t)
    {
        if (projectorsLight != null)
        {
            projectorsLight.intensity = lightVal;
        }

        if (volumeCons != null)
        {
            for (int i = 0; i < volumeCons.Length; i++)
            {
                if (volumeCons[i] != null && i < startConeVals.Length)
                {
                    volumeCons[i].intensityGlobal = Mathf.Lerp(startConeVals[i], targetConeVal, t);
                }
            }
        }
    }

    private void ApplyIntensitiesToInitial(float startLightVal, float[] startConeVals, float t)
    {
        if (projectorsLight != null)
        {
            projectorsLight.intensity = Mathf.Lerp(startLightVal, _initialLightIntensity, t);
        }

        if (volumeCons != null && _initialConeIntensities != null)
        {
            for (int i = 0; i < volumeCons.Length; i++)
            {
                if (volumeCons[i] != null && i < startConeVals.Length && i < _initialConeIntensities.Length)
                {
                    volumeCons[i].intensityGlobal = Mathf.Lerp(startConeVals[i], _initialConeIntensities[i], t);
                }
            }
        }
    }
}