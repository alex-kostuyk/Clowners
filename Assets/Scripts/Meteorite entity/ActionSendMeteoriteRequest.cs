using System.Collections;
using UnityEngine;

public class ActionSendMeteoriteRequest : MonoBehaviour, IAction
{
    [SerializeField]
    private RequestType type;

    private Coroutine _actionRoutine;

    public void StartAction()
    {
        if (_actionRoutine != null)
        {
            StopCoroutine(_actionRoutine);
        }

        _actionRoutine = StartCoroutine(ProcessActionRoutine());
    }

    private IEnumerator ProcessActionRoutine()
    {
        bool isCorrect = false;

        if (MeteoriteEntity.Instance != null )
        {
            isCorrect = MeteoriteEntity.Instance.IsThisTypeCorrect(type);
        }

        float delay = isCorrect
            ? Constants.TimeToReportIfActionIsCorrect
            : Constants.TimeToReportIfActionIsWrong;

        yield return new WaitForSeconds(delay);

        if (MeteoriteEntity.Instance != null)
        {
            MeteoriteEntity.Instance.TryToSatisfyRequest(type);
        }

        _actionRoutine = null;
    }

    private void OnDisable()
    {
        if (_actionRoutine != null)
        {
            StopCoroutine(_actionRoutine);
            _actionRoutine = null;
        }
    }
}