using UnityEngine;

public class ActionSendMeteoriteRequest : MonoBehaviour, IAction
{
    [SerializeField]
    private RequestType type;

    public void StartAction()
    {
        MeteoriteEntity.Instance.TryToSatisfyRequest(type);
    }
}
