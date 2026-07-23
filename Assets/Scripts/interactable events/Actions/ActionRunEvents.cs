using System.Collections;
using UnityEngine;

public class ActionRunEvents : MonoBehaviour, IAction
{
    [SerializeField] private Event[] eventsToRun;
    [SerializeField] private float delay;

    public  void StartAction()
    {
        if(delay<=0)
        {
            runEvents();
            return;
        }

        StartCoroutine(delayAction());
    }

    private IEnumerator delayAction()
    {      
        yield return new WaitForSeconds(delay);

        runEvents();
        
    }

    private void runEvents()
    {
        foreach (Event currentEvent in eventsToRun)
        {
            if (currentEvent != null && currentEvent.gameObject.activeSelf)
            {
                currentEvent.CallActions();
            }
        }
    }
}
