using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionForceStandUp : MonoBehaviour,IAction
{
    [SerializeField]
    private float duration = 1.5f;
    
    public void StartAction()
    {
       // StartCoroutine(enumerator());
    }

   /* private IEnumerator enumerator()
    {

        FP_Controller fP_Controller = FindObjectOfType<FP_Controller>();

        if(fP_Controller == null || !fP_Controller.IsCrouching() )
            yield break;

        float startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            fP_Controller.StandUp();
            yield return null;
        }
    }*/
}
