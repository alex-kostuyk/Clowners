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

    private HashSet<RequestType> _availableFoodTypes = new HashSet<RequestType>();

    private readonly HashSet<RequestType> _foodRequestTypes = new HashSet<RequestType>
    {
        RequestType.Praise,
        RequestType.Pizza,
        RequestType.Burger,
        RequestType.Beer,
        RequestType.Cigarettes,
        RequestType.Watermelon,
        RequestType.Apple
    };

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
        CacheAvailableFoodInScene();
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

    public bool IsThisTypeCorrect(RequestType type) => (type == _currentRequest.RequestType);

    public void TryToSatisfyRequest(RequestType type)
    {
        if (_currentRequest == null) return;

        if (type == _currentRequest.RequestType)
        {
            _stability = Mathf.Clamp01(_stability + _correctAnswerBonus);
            _onRightAnswer?.Invoke();

            OnStabilityChange?.Invoke(_stability);

            if (!CheckInstability())
            {
                AssignRandomRequest();
            }
        }
        else
        {
            _stability = Mathf.Clamp01(_stability - _wrongAnswerPenalty);
            _onWrongAnswer?.Invoke();

            OnStabilityChange?.Invoke(_stability);

            CheckInstability();
            // Request is NOT changed on wrong answer
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

    private void CacheAvailableFoodInScene()
    {
        _availableFoodTypes.Clear();
        MeteoriteFoodTag[] foodTags = FindObjectsOfType<MeteoriteFoodTag>();

        foreach (var food in foodTags)
        {
            if (food != null)
            {
                _availableFoodTypes.Add(food.FoodType);
            }
        }
    }

    private void AssignRandomRequest()
    {
        if (RequestProperties == null || RequestProperties.Length == 0) return;

        CacheAvailableFoodInScene();

        List<RequestProperty> validRequests = new List<RequestProperty>();

        foreach (var req in RequestProperties)
        {
            // Check food availability constraint
            if (_foodRequestTypes.Contains(req.RequestType))
            {
                if (_availableFoodTypes.Contains(req.RequestType))
                {
                    validRequests.Add(req);
                }
            }
            else
            {
                // Non-food requests are always valid
                validRequests.Add(req);
            }
        }

        if (validRequests.Count == 0) return;

        // Filter out the current request so it can't repeat consecutively
        List<RequestProperty> nonRepeatingRequests = new List<RequestProperty>();
        if (_currentRequest != null)
        {
            foreach (var req in validRequests)
            {
                if (req.RequestType != _currentRequest.RequestType)
                {
                    nonRepeatingRequests.Add(req);
                }
            }
        }

        // Use the filtered list if we have other options; otherwise fall back to validRequests
        List<RequestProperty> poolToPickFrom = nonRepeatingRequests.Count > 0 ? nonRepeatingRequests : validRequests;

        int randomIndex = UnityEngine.Random.Range(0, poolToPickFrom.Count);
        _currentRequest = poolToPickFrom[randomIndex];

        Debug.Log(_currentRequest.RequestType.ToString());
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