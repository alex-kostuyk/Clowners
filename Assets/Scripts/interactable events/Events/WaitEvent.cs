
using UnityEngine;

public class WaitEvent : Event
{
    private CameraLookAtEventTrigger cameraLookAtEventTrigger;

    public void Start()
    {

        Invoke("innit", 1);
    }

    private void innit()
    {
        cameraLookAtEventTrigger = FindObjectOfType<CameraLookAtEventTrigger>(true);
    }

    public override void Action()
    {
        if (cameraLookAtEventTrigger == null)
            innit();

        if (cameraLookAtEventTrigger.IsActionReady())
        {
            CallActions();
        }
    }

    public override bool IsWaitEvent()
    {
        return true;
    }
}
