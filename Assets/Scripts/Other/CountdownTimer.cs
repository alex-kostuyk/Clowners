using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Starting time in seconds")]
    [SerializeField] private float startTime = 120f;
    [Tooltip("Show centiseconds (MM:SS:ms) instead of just MM:SS")]
    [SerializeField] private bool showMilliseconds = true;

    [Header("References")]
    [Tooltip("3D TextMeshPro component to display the countdown. Auto-resolved if left empty.")]
    [SerializeField] private TextMeshPro display;

    [Header("Events")]
    public UnityEvent OnCountdownFinished;

    private float _timeRemaining;
    private bool _isRunning;
    private bool _hasFinished;

    private void Awake()
    {
        if (display == null)
            display = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        StartCountdown();
    }

    /// <summary>Starts or restarts the countdown from <see cref="startTime"/>.</summary>
    public void StartCountdown()
    {
        _timeRemaining = startTime;
        _isRunning = true;
        _hasFinished = false;
        UpdateDisplay();
    }

    /// <summary>Starts or restarts the countdown with a custom duration.</summary>
    public void StartCountdown(float duration)
    {
        startTime = duration;
        StartCountdown();
    }

    /// <summary>Pauses the countdown.</summary>
    public void Pause() => _isRunning = false;

    /// <summary>Resumes a paused countdown.</summary>
    public void Resume() => _isRunning = true;

    private void Update()
    {
        if (!_isRunning || _hasFinished)
            return;

        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            _isRunning = false;
            _hasFinished = true;
            UpdateDisplay();
            OnCountdownFinished?.Invoke();
            return;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (display == null) return;

        int minutes = Mathf.FloorToInt(_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60f);

        if (showMilliseconds)
        {
            int centiseconds = Mathf.FloorToInt((_timeRemaining - Mathf.Floor(_timeRemaining)) * 100f);
            display.text = $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
        }
        else
        {
            display.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
