using UnityEngine;

public class ActionCallEventWithInterval : MonoBehaviour, IAction
{
    [SerializeField]
    private Event [] targetEvents;
    [SerializeField]
    private MonoBehaviour [] targetActions;
    [SerializeField]
    private float interval = 0.1f;
    private float lastTimeCalled = 0;

    public void StartAction() 
    {
        if (lastTimeCalled+interval < Time.time)
        {
           
            foreach (Event value in targetEvents) 
            {
                value?.CallActions();
            }

            foreach (IAction action in targetActions)
            {
                action?.StartAction();
            }

            lastTimeCalled = Time.time;
        }
    }
}
