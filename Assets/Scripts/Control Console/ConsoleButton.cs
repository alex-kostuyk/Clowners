using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConsoleButton : MonoBehaviour
{
    [SerializeField]
    private RequestType _requestType;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private UnityEvent _onPressed;

    [Header("Shader Property Names")]
    [SerializeField]
    private string _colorPropertyName = "_Color";
    [SerializeField]
    private string _emissionColorPropertyName = "_EmissionColor";

    [Header("Deactivation Settings")]
    [SerializeField, Range(0f, 1f)]
    private float _deactivatedDarknessFactor = 0.25f;

    private Color _baseColor;
    private bool _isActive = true;
    private MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();

        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Start()
    {
        InitializeColor();
    }

    private void OnEnable()
    {
        ControlConsoles.OnActivate += HandleActivate;
        ControlConsoles.OnDeactivate += HandleDeactivate;
    }

    private void OnDisable()
    {
        ControlConsoles.OnActivate -= HandleActivate;
        ControlConsoles.OnDeactivate -= HandleDeactivate;
    }

    private void InitializeColor()
    {
        if (MeteoriteEntity.Instance == null) return;

        foreach (var prop in MeteoriteEntity.Instance.RequestProperties)
        {
            if (prop.RequestType == _requestType)
            {
                _baseColor = prop.RequestColor;
                UpdateVisualState(_isActive);
                break;
            }
        }
    }

    public void PressButton()
    {
        if (_isActive)
        {
            _onPressed?.Invoke();
        }
    }

    private void HandleActivate()
    {
        _isActive = true;
        UpdateVisualState(true);
    }

    private void HandleDeactivate()
    {
        _isActive = false;
        UpdateVisualState(false);
    }

    private void UpdateVisualState(bool active)
    {
        if (_meshRenderer == null) return;

        _meshRenderer.GetPropertyBlock(_propBlock);

        if (active)
        {
            _propBlock.SetColor(_colorPropertyName, _baseColor);
            _propBlock.SetColor(_emissionColorPropertyName, _baseColor);
        }
        else
        {
            Color darkerColor = _baseColor * _deactivatedDarknessFactor;
            darkerColor.a = _baseColor.a;

            _propBlock.SetColor(_colorPropertyName, darkerColor);
            _propBlock.SetColor(_emissionColorPropertyName, Color.black);
        }

        _meshRenderer.SetPropertyBlock(_propBlock);
    }
}