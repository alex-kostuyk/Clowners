using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heater : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    [SerializeField]
    private float _lerpSpeed = 2f;

    [ColorUsage(true, true)]
    [SerializeField]
    private Color _emissionTargetColor = Color.red;

    [SerializeField]
    private float _emissionBrightnessMod = 2f;

    
    private string _emissionColorPropertyName = "_EmissionColor";

    private Coroutine _lerpRoutine;
    private Color _currentEmissionColor = Color.black;

    private void Start()
    {
        _material.SetColor(_emissionColorPropertyName, Color.black);
    }

    public void Activate()
    {
        Color finalTargetColor = _emissionTargetColor * _emissionBrightnessMod;
        StartEmissionLerp(finalTargetColor);
    }

    public void Deactivate()
    {
        StartEmissionLerp(Color.black);
    }

    private void StartEmissionLerp(Color targetColor)
    {
        if (_lerpRoutine != null)
        {
            StopCoroutine(_lerpRoutine);
        }

        _lerpRoutine = StartCoroutine(LerpEmissionRoutine(targetColor));
    }

    private IEnumerator LerpEmissionRoutine(Color targetColor)
    {
        if (_material == null) yield break;

        while (Vector4.Distance(_currentEmissionColor, targetColor) > 0.01f)
        {
            _currentEmissionColor = Color.Lerp(_currentEmissionColor, targetColor, Time.deltaTime * _lerpSpeed);
            _material.SetColor(_emissionColorPropertyName, _currentEmissionColor);
            yield return null;
        }

        _currentEmissionColor = targetColor;
        _material.SetColor(_emissionColorPropertyName, _currentEmissionColor);
        _lerpRoutine = null;
    }
}