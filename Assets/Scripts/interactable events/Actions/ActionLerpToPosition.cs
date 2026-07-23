using System.Collections;
using UnityEngine;

public class ActionLerpToPosition : MonoBehaviour, IAction
{
    [SerializeField] private Transform target, transformToLerp,startPosition;
    [SerializeField] private float seconds = 3;

    public  void StartAction()
    {
        if(startPosition!=null)
            transformToLerp.position = startPosition.position;

        StartCoroutine(lerp());
    }
    
    private IEnumerator lerp()
    {      

        if(transformToLerp == null)
            yield break;

        float elapsedTime = 0f;
        Vector3 initialPosition = transformToLerp.position;
        Quaternion initialRotation = transformToLerp.rotation;
         Vector3 initialScale = transformToLerp.localScale;
        
        while (elapsedTime < seconds)
        {
            float t = elapsedTime / seconds;
            transformToLerp.position = Vector3.Lerp(initialPosition, target.position, t);
            transformToLerp.rotation = Quaternion.Lerp(initialRotation, target.rotation, t);
            transformToLerp.localScale = Vector3.Lerp(initialScale, target.localScale, t);

            elapsedTime += Time.deltaTime;
            yield return  new WaitForFixedUpdate(); 
        }

    
        transformToLerp.position = target.position;
        transformToLerp.rotation = target.rotation;
    }

    public void SetTarget(Transform targetValue)
    {
        target = targetValue;
    }
        
    
    
}

