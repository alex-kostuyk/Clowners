using System.Collections;
using UnityEngine;

public class ActionStartActionWithDelay : MonoBehaviour, IAction
{
    [SerializeField] private MonoBehaviour[] actionsToRun;
    [SerializeField] private float delay;

    public  void StartAction(){
        StartCoroutine(delayAction());
    }

    private IEnumerator delayAction()
    {
        if (delay <= 0)
            yield return new WaitForFixedUpdate();
        else 
            yield return new WaitForSecondsRealtime(delay);

         foreach (IAction currentAction in actionsToRun)
        {
            if (currentAction != null)
            {  
                currentAction.StartAction();
            }
        }
    }
}