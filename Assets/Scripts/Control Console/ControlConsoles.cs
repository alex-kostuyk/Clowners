using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ControlConsoles : MonoBehaviour
{
    public static ControlConsoles Instance { get; private set; }

    public static event Action OnDeactivate;
    public static event Action<int> OnCountdownTick;
    public static event Action OnActivate;

    [SerializeField]
    private Transform[] _buttonsPositions;

    [SerializeField]
    private int _cooldownDuration = 7;

    private Coroutine _timerCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        shuffleButtonsPositions();
    }

    private void OnValidate()
    {
        if(_buttonsPositions.Length > 0)
            return;

        _buttonsPositions = GetComponentsInChildren<ConsoleButton>(true)
            .Select(button => button.transform)
            .ToArray();
    }

    public void OnButtonPressed()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }

        _timerCoroutine = StartCoroutine(CooldownRoutine(_cooldownDuration));
    }

    private IEnumerator CooldownRoutine(int totalSeconds)
    {
        OnDeactivate?.Invoke();

        for (int secondsLeft = totalSeconds; secondsLeft > 0; secondsLeft--)
        {
            OnCountdownTick?.Invoke(secondsLeft);
            yield return new WaitForSeconds(1f);
        }

        OnActivate?.Invoke();
        _timerCoroutine = null;
    }

    private void shuffleButtonsPositions()

    {

        if (_buttonsPositions == null || _buttonsPositions.Length < 2) return;



        for (int i = _buttonsPositions.Length - 1; i > 0; i--)

        {

            int randomIndex = UnityEngine.Random.Range(0, i + 1);



            Vector3 tempPosition = _buttonsPositions[i].position;

            _buttonsPositions[i].position = _buttonsPositions[randomIndex].position;

            _buttonsPositions[randomIndex].position = tempPosition;

        }

    }


}