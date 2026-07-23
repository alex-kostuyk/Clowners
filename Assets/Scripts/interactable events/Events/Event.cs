
using UnityEngine;


public class Event : MonoBehaviour
{ 
    [SerializeField] private MonoBehaviour[] actionsToPerform;
    [SerializeField] private bool actionOnStart = false,actionsOnDisable;
    public string Discription;
    public bool limitForHeavy;
    private void OnEnable()
    {
        if(actionOnStart)
            CallActions();

    }

    private void OnDisable()
    {
        if(actionsOnDisable)
            CallActions();
    }

    public virtual void Action()
    {
        if(gameObject.activeInHierarchy)
            CallActions();

    }

    public virtual bool IsWaitEvent()
    {
        return false;
    }

    public void CallActions()
    {
        foreach (IAction action in actionsToPerform)
        {
                action?.StartAction();
        }
    }

   

    
}
