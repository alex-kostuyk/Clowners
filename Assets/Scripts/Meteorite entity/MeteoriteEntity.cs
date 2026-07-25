using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeteoriteEntity : MonoBehaviour
{
    public static event Action<float> OnStabilityChange;
    public static event Action<RequestProperty> OnRequestChange;

    public static MeteoriteEntity Instance { get; private set; }

    public RequestProperty[] RequestProperties;

    [SerializeField]
    private UnityEvent _onWrongAnswer, _onRightAnswer, _onToInstable;

    [Header("Stability Settings")]
    [SerializeField, Range(0f, 1f)]
    private float _stability = 1f;
    [SerializeField]
    private float _decayRate = 0.05f; // Amount lost per second
    [SerializeField]
    private float _correctAnswerBonus = 0.15f; // Amount gained on correct answer
    [SerializeField]
    private float _wrongAnswerPenalty = 0.20f; // Extra penalty on wrong answer

    private RequestProperty _currentRequest;
    private bool _isInstableTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AssignRandomRequest();
    }

    private void FixedUpdate()
    {
        DecreaseStabilityOverTime();
    }

    private void DecreaseStabilityOverTime()
    {
        if (_stability <= 0) return;

        _stability -= _decayRate * Time.fixedDeltaTime;
        _stability = Mathf.Clamp01(_stability);

        OnStabilityChange?.Invoke(_stability);

        CheckInstability();
    }

    public void TryToSatisfyRequest(RequestType type)
    {
        if (_currentRequest == null) return;

        if (type == _currentRequest.RequestType)
        {
            _stability = Mathf.Clamp01(_stability + _correctAnswerBonus);
            _onRightAnswer?.Invoke();
        }
        else
        {
            _stability = Mathf.Clamp01(_stability - _wrongAnswerPenalty);
            _onWrongAnswer?.Invoke();
        }

        OnStabilityChange?.Invoke(_stability);

        if (!CheckInstability())
        {
            AssignRandomRequest();
        }
    }

    private bool CheckInstability()
    {
        if (_stability <= 0f && !_isInstableTriggered)
        {
            _isInstableTriggered = true;
            _onToInstable?.Invoke();
            return true;
        }
        return false;
    }

    private void AssignRandomRequest()
    {
        if (RequestProperties == null || RequestProperties.Length == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, RequestProperties.Length);
        _currentRequest = RequestProperties[randomIndex];

        OnRequestChange?.Invoke(_currentRequest);
    }
}

[Serializable]
public class RequestProperty
{
    public Color RequestColor;
    public RequestType RequestType;
}

public enum RequestType
{
    TurnOnTheLight,
    TurnOffTheLight,
    Cooling,
    Heating,
    ShowBrainrot,
    ShowNonFiction,
    Insult,
    Praise,
    Pizza,
    Burger,
    Beer,
    Cigarettes,
    Watermelon,
    Apple
}