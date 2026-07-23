using UnityEngine;
using UnityEngine.Events;

public class ActionRunUnityEvents : MonoBehaviour, IAction
{
    [SerializeField]
    private UnityEvent unityEvent;
    public void StartAction()
    {
        unityEvent?.Invoke();
    }
}
