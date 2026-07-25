using UnityEngine;
using UnityEngine.UI;

public class MenuCameraOrbit : MonoBehaviour
{
    [System.Serializable]
    public class CameraShot
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3 lookAtOffset = Vector3.zero; // offset from target to look at
    }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Shots")]
    [SerializeField] private CameraShot[] shots;

    [Header("Timing")]
    [SerializeField] private float panDuration = 5f;
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Camera")]
    [SerializeField] private float fieldOfView = 65f;

    [Header("Fade")]
    [SerializeField] private Image fadeOverlay;

    private Camera _camera;
    private int _currentShot;
    private float _shotTimer;

    private enum State { FadeIn, Panning, FadeOut }
    private State _state;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        if (_camera != null)
            _camera.fieldOfView = fieldOfView;

        if (shots == null || shots.Length == 0)
        {
            // Default shots: lateral arcs at constant ~4m radius from target
            shots = new CameraShot[]
            {
                new CameraShot { startPosition = new Vector3(4f, 1.5f, 0f), endPosition = new Vector3(3f, 1.5f, 2.6f), lookAtOffset = Vector3.up * 0.5f },
                new CameraShot { startPosition = new Vector3(-2.8f, 2.2f, 2.8f), endPosition = new Vector3(-4f, 2.0f, 0f), lookAtOffset = Vector3.up * 0.3f },
                new CameraShot { startPosition = new Vector3(0f, 0.8f, -4f), endPosition = new Vector3(2.8f, 1.0f, -2.8f), lookAtOffset = Vector3.zero },
                new CameraShot { startPosition = new Vector3(-2.8f, 3.0f, -2.8f), endPosition = new Vector3(-4f, 2.8f, 0f), lookAtOffset = Vector3.up * 0.8f },
            };
        }

        _currentShot = 0;
        _state = State.FadeIn;
        _shotTimer = 0f;
        SetFade(1f); // start fully black
        ApplyShotPosition(0f);
    }

    private void LateUpdate()
    {
        if (target == null || shots == null || shots.Length == 0) return;

        _shotTimer += Time.deltaTime;

        switch (_state)
        {
            case State.FadeIn:
                float fadeInT = Mathf.Clamp01(_shotTimer / fadeDuration);
                SetFade(1f - fadeInT);
                ApplyShotPosition(0f);
                if (fadeInT >= 1f)
                {
                    _state = State.Panning;
                    _shotTimer = 0f;
                }
                break;

            case State.Panning:
                float panT = Mathf.Clamp01(_shotTimer / panDuration);
                float smoothT = panT * panT * (3f - 2f * panT); // smoothstep
                ApplyShotPosition(smoothT);
                if (panT >= 1f)
                {
                    _state = State.FadeOut;
                    _shotTimer = 0f;
                }
                break;

            case State.FadeOut:
                float fadeOutT = Mathf.Clamp01(_shotTimer / fadeDuration);
                SetFade(fadeOutT);
                ApplyShotPosition(1f);
                if (fadeOutT >= 1f)
                {
                    _currentShot = (_currentShot + 1) % shots.Length;
                    _state = State.FadeIn;
                    _shotTimer = 0f;
                    ApplyShotPosition(0f);
                }
                break;
        }
    }

    private void ApplyShotPosition(float t)
    {
        var shot = shots[_currentShot];
        Vector3 worldStart = target.position + shot.startPosition;
        Vector3 worldEnd = target.position + shot.endPosition;

        transform.position = Vector3.Lerp(worldStart, worldEnd, t);
        transform.LookAt(target.position + shot.lookAtOffset);
    }

    private void SetFade(float alpha)
    {
        if (fadeOverlay == null) return;
        var c = fadeOverlay.color;
        c.a = alpha;
        fadeOverlay.color = c;
    }
}
