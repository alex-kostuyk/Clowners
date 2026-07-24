using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ConsoleScreen : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onActionsDischarge, _onActionsCharge;
    [SerializeField]
    private Slider _rechargingSlider;
    [SerializeField]
    private Slider _entityStateSlider;
    [SerializeField]
    private TextMeshProUGUI _secondsLeftText;
    [SerializeField]
    private TextMeshProUGUI _binaryText;
    [SerializeField]
    private float _binaryUpdateInterval = 0.1f;

    private int _maxSeconds;
    private Coroutine _binaryRoutine;

    private void OnEnable()
    {
        ControlConsoles.OnActivate += HandleActivate;
        ControlConsoles.OnDeactivate += HandleDeactivate;
        ControlConsoles.OnCountdownTick += HandleCountdownTick;

        if (_binaryText != null)
        {
            _binaryRoutine = StartCoroutine(GenerateBinaryRoutine());
        }
    }

    private void OnDisable()
    {
        ControlConsoles.OnActivate -= HandleActivate;
        ControlConsoles.OnDeactivate -= HandleDeactivate;
        ControlConsoles.OnCountdownTick -= HandleCountdownTick;

        if (_binaryRoutine != null)
        {
            StopCoroutine(_binaryRoutine);
            _binaryRoutine = null;
        }
    }

    private System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder(25);

    private IEnumerator GenerateBinaryRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_binaryUpdateInterval);

        while (true)
        {
            _stringBuilder.Clear();
            for (int i = 0; i < 25; i++)
            {
                _stringBuilder.Append(Random.value > 0.5f ? '1' : '0');
            }

            _binaryText.text = _stringBuilder.ToString();
            yield return wait;
        }
    }

    private void HandleActivate()
    {
        _onActionsCharge?.Invoke();
    }

    private void HandleDeactivate()
    {
        _maxSeconds = 0;
        _onActionsDischarge?.Invoke();
    }

    private void HandleCountdownTick(int secondsLeft)
    {
        if (_secondsLeftText != null)
        {
            _secondsLeftText.text = secondsLeft + " sec.";
        }

        if (secondsLeft > _maxSeconds)
        {
            _maxSeconds = secondsLeft;
            if (_rechargingSlider != null)
            {
                _rechargingSlider.maxValue = _maxSeconds;
            }
        }

        if (_rechargingSlider != null)
        {
            _rechargingSlider.value = secondsLeft;
        }
    }
}