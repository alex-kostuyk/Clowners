using System.Collections;
using UnityEngine;

public class ActionCameraShake : MonoBehaviour, IAction
{

    [Header("Shake Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float speed = 25f;
    [SerializeField] private float positionStrength = 0.3f;
    [SerializeField] private float angleStrength = 2.0f;

    private Camera targetCamera;
    private Coroutine _shakeCoroutine;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private WaitForFixedUpdate wait;

    private void Awake()
    {
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Start()
    {
        wait = new WaitForFixedUpdate();
    }

    public void StartAction()
    {
        if (targetCamera == null)
        {
            Awake();
            return;
        }

        // Restart coroutine if one is already playing
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            targetCamera.transform.localPosition = initialLocalPosition;
            targetCamera.transform.localRotation = initialLocalRotation;
            initialLocalPosition = Vector3.zero;
            initialLocalRotation = Quaternion.identity;
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Transform camTransform = targetCamera.transform;

        // Cache original local offsets so we can reset cleanly when done
        initialLocalPosition = camTransform.localPosition;
        initialLocalRotation = camTransform.localRotation;

        float elapsed = 0f;

        // Seed Perlin noise randomly so each shake pattern feels unique
        float noiseSeedX = Random.Range(0f, 100f);
        float noiseSeedY = Random.Range(0f, 100f);
        float noiseSeedZ = Random.Range(0f, 100f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Fade out the intensity over the shake duration (1.0 -> 0.0)
            float damper = 1f - Mathf.Clamp01(elapsed / duration);
            float timeFactor = Time.time * speed;

            // Generate smooth Perlin Noise values mapped from [-1, 1]
            float posX = (Mathf.PerlinNoise(noiseSeedX, timeFactor) * 2f - 1f) * positionStrength * damper;
            float posY = (Mathf.PerlinNoise(noiseSeedY, timeFactor) * 2f - 1f) * positionStrength * damper;
            float posZ = (Mathf.PerlinNoise(noiseSeedZ, timeFactor) * 2f - 1f) * positionStrength * damper;

            float rotX = (Mathf.PerlinNoise(noiseSeedZ, timeFactor) * 2f - 1f) * angleStrength * damper;
            float rotY = (Mathf.PerlinNoise(noiseSeedX, timeFactor) * 2f - 1f) * angleStrength * damper;
            float rotZ = (Mathf.PerlinNoise(noiseSeedY, timeFactor) * 2f - 1f) * angleStrength * damper;

            // Apply offsets to the child Camera object
            camTransform.localPosition = initialLocalPosition + new Vector3(posX, posY, posZ);
            camTransform.localRotation = initialLocalRotation * Quaternion.Euler(rotX, rotY, rotZ);

            yield return wait;
        }

        // Reset back to original position/rotation smoothly
        camTransform.localPosition = initialLocalPosition;
        camTransform.localRotation = initialLocalRotation;

        initialLocalPosition = Vector3.zero;
        initialLocalRotation = Quaternion.identity;

        _shakeCoroutine = null;
    }
}