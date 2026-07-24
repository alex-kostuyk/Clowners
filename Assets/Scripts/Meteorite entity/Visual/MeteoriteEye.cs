using KinematicCharacterController.Examples;
using System.Collections;
using UnityEngine;

public class MeteoriteEye : MonoBehaviour
{
    public Transform Target;
    public Vector3 TargetOffset;

    [SerializeField] private float _stepDuration = 0.05f;
    [SerializeField] private float _pauseDuration = 0.3f;
    [SerializeField] private float _jitterAngle = 1f;

    [SerializeField] private float _idleAngleRange = 15f;
    [SerializeField] private float _detectionAngle = 60f;
    [SerializeField, Range(0f, 1f)] private float _idleMoveChance = 0.2f;

    private Quaternion _initialRotation;
    private Quaternion _currentIdleRotation;
    private ExampleCharacterController _player;

    private void Start()
    {
        _initialRotation = transform.rotation;
        _currentIdleRotation = _initialRotation;
        _player = Object.FindFirstObjectByType<ExampleCharacterController>();

        StartCoroutine(EyeStepRoutine());
    }

    private IEnumerator EyeStepRoutine()
    {
        while (true)
        {
            bool hasNonPlayerTarget = Target != null && (_player == null || Target != _player.transform);

            if (!hasNonPlayerTarget)
            {
                Target = GetPlayerInAngle();
            }

            Quaternion targetRotation;

            if (Target != null)
            {
                Vector3 targetPosition = Target.position + TargetOffset;
                Vector3 directionToTarget = targetPosition - transform.position;

                if (directionToTarget != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                }
                else
                {
                    targetRotation = transform.rotation;
                }
            }
            else
            {
                if (Random.value <= _idleMoveChance)
                {
                    float randomY = Random.Range(-_idleAngleRange, _idleAngleRange);
                    float randomX = Random.Range(-_idleAngleRange * 0.5f, _idleAngleRange * 0.5f);
                    _currentIdleRotation = _initialRotation * Quaternion.Euler(randomX, randomY, 0);
                }
                targetRotation = _currentIdleRotation;
            }

            if (_jitterAngle > 0f)
            {
                Vector3 jitterOffset = Random.insideUnitSphere * _jitterAngle;
                targetRotation *= Quaternion.Euler(jitterOffset);
            }

            Quaternion startRotation = transform.rotation;
            float elapsed = 0f;

            while (elapsed < _stepDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _stepDuration;
                float stepProgress = Mathf.Sin(t * Mathf.PI * 0.5f);

                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, stepProgress);
                yield return null;
            }

            transform.rotation = targetRotation;

            yield return new WaitForSeconds(_pauseDuration);
        }
    }

    private Transform GetPlayerInAngle()
    {
        if (_player == null) return null;

        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0;

        Vector3 forward = _initialRotation * Vector3.forward;
        forward.y = 0;

        if (Vector3.Angle(forward, direction) <= _detectionAngle)
        {
            return _player.transform;
        }

        return null;
    }
}