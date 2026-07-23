using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent  : Event
{
  
    [SerializeField] private UnityEvent _onEnterEvent,_onExitEvent;

 
    private void OnTriggerEnter(Collider other) 
    {
        if (checkForPlayer(other))
            _onEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other) 
    {
        if(checkForPlayer(other))
            _onExitEvent?.Invoke();
    }

    private bool checkForPlayer(Collider other)
    {
        return other.GetComponent<ExampleCharacterController>() != null;
    }

    public override bool IsWaitEvent()
    {
        return false;
    }
}
