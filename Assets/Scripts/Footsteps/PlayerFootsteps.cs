using UnityEngine;
using KinematicCharacterController.Examples;

namespace Footsteps
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [SerializeField] private ExampleCharacterController _characterController;
        [SerializeField] private AudioSource _audioSource;

        [Header("Default Profile")]
        [SerializeField] private FootstepSurfaceProfile _defaultProfile;

        [Header("Surface Profiles")]
        [SerializeField] private FootstepSurfaceProfile[] _surfaceProfiles;

        [Header("Distance & Pitch")]
        [SerializeField] private float _stepInterval = 1.8f;
        [SerializeField] private float _minPitch = 0.9f;
        [SerializeField] private float _maxPitch = 1.1f;

        [Header("Ladder Settings")]
        [SerializeField] private FootstepSurfaceProfile _ladderProfile;
        [SerializeField] private float _climbStepInterval = 1.0f;

        private float _distanceWalked;
        private float _climbDistanceWalked;

        private void Reset()
        {
            _audioSource = GetComponent<AudioSource>();
            _characterController = GetComponentInChildren<ExampleCharacterController>();
            if (_characterController == null)
            {
                _characterController = GetComponentInParent<ExampleCharacterController>();
            }
        }

        private void OnEnable()
        {
            if (_characterController != null)
            {
                _characterController.OnCharacterLanded += HandleLanded;
                _characterController.OnCharacterLeftGround += HandleLeftGround;
            }
        }

        private void OnDisable()
        {
            if (_characterController != null)
            {
                _characterController.OnCharacterLanded -= HandleLanded;
                _characterController.OnCharacterLeftGround -= HandleLeftGround;
            }
        }

        private void Update()
        {
            if (_characterController == null) return;

            switch (_characterController.CurrentCharacterState)
            {
                case CharacterState.Climbing:
                    _distanceWalked = 0f;
                    float climbSpeed = _characterController.Motor.Velocity.magnitude;
                    _climbDistanceWalked += climbSpeed * Time.deltaTime;
                    if (_climbDistanceWalked >= _climbStepInterval)
                    {
                        _climbDistanceWalked = 0f;
                        PlayClimbStepSound();
                    }
                    break;

                case CharacterState.Default:
                    _climbDistanceWalked = 0f;
                    if (_characterController.Motor.GroundingStatus.IsStableOnGround)
                    {
                        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_characterController.Motor.Velocity, _characterController.Motor.CharacterUp);
                        _distanceWalked += horizontalVelocity.magnitude * Time.deltaTime;

                        if (_distanceWalked >= _stepInterval)
                        {
                            _distanceWalked = 0f;
                            PlayStepSound();
                        }
                    }
                    else
                    {
                        _distanceWalked = 0f;
                    }
                    break;
            }
        }

        private FootstepSurfaceProfile GetMatchingProfile(Collider groundCollider)
        {
            if (groundCollider != null && _surfaceProfiles != null)
            {
                foreach (var profile in _surfaceProfiles)
                {
                    if (profile != null && !string.IsNullOrEmpty(profile.TagName) && groundCollider.CompareTag(profile.TagName))
                    {
                        return profile;
                    }
                }
            }
            return _defaultProfile;
        }

        private void PlayStepSound()
        {
            Collider groundCollider = _characterController.Motor.GroundingStatus.GroundCollider;
            FootstepSurfaceProfile profile = GetMatchingProfile(groundCollider);

            AudioClip[] clips = (profile != null && profile.WalkClips != null && profile.WalkClips.Length > 0)
                ? profile.WalkClips
                : (_defaultProfile != null ? _defaultProfile.WalkClips : null);

            PlayRandomClip(clips);
        }

        private void PlayClimbStepSound()
        {
            FootstepSurfaceProfile profile = _ladderProfile != null ? _ladderProfile : _defaultProfile;
            AudioClip[] clips = (profile != null && profile.WalkClips != null && profile.WalkClips.Length > 0)
                ? profile.WalkClips
                : (_defaultProfile != null ? _defaultProfile.WalkClips : null);

            PlayRandomClip(clips);
        }

        private void HandleLanded()
        {
            Collider groundCollider = _characterController.Motor.GroundingStatus.GroundCollider;
            FootstepSurfaceProfile profile = GetMatchingProfile(groundCollider);

            AudioClip[] clips = (profile != null && profile.LandClips != null && profile.LandClips.Length > 0)
                ? profile.LandClips
                : (_defaultProfile != null ? _defaultProfile.LandClips : null);

            PlayRandomClip(clips);
        }

        private void HandleLeftGround()
        {
            Collider groundCollider = _characterController.Motor.GroundingStatus.GroundCollider;
            FootstepSurfaceProfile profile = GetMatchingProfile(groundCollider);

            AudioClip[] clips = (profile != null && profile.JumpClips != null && profile.JumpClips.Length > 0)
                ? profile.JumpClips
                : (_defaultProfile != null ? _defaultProfile.JumpClips : null);

            PlayRandomClip(clips);
        }

        private void PlayRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0 || _audioSource == null) return;

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
            {
                _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}
