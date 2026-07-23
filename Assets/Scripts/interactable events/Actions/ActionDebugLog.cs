using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDebugLog : MonoBehaviour, IAction
{
    public string LogText;
    public bool LogAsError = false;

    public void StartAction()
    {
        if (LogAsError)
            Debug.LogError(LogText);
        else
            Debug.Log(LogText);
    }
}
