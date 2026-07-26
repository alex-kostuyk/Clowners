using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapToolRotation : MonoBehaviour
{
    public Transform Wheel;
    public float seconds = 10f;

    [SerializeField]
    private float _totalRotations = 8f; // Number of full 360 degree rotations
    [SerializeField]
    private Vector3 _rotationAxis = Vector3.forward; // Local rotation axis

    private Coroutine _rotationRoutine;

    public void StartRotation()
    {
        if (Wheel == null) return;

        if (_rotationRoutine != null)
        {
            StopCoroutine(_rotationRoutine);
        }

        _rotationRoutine = StartCoroutine(RotateWheelRoutine());
    }

    private IEnumerator RotateWheelRoutine()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = Wheel.localRotation;
        float totalDegrees = _totalRotations * 360f;

        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / seconds);

            // SmoothStart and SmoothEnd via SmoothStep
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            float currentDegrees = smoothT * totalDegrees;
            Wheel.localRotation = startRotation * Quaternion.AngleAxis(currentDegrees, _rotationAxis);

            yield return null;
        }

        Wheel.localRotation = startRotation * Quaternion.AngleAxis(totalDegrees, _rotationAxis);
        _rotationRoutine = null;
    }
}