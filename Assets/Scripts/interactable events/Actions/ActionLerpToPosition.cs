using System.Collections;
using UnityEngine;

public class ActionLerpToPosition : MonoBehaviour, IAction
{
    [SerializeField] private Transform targetPosition, transformToLerp,startPosition;
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
            transformToLerp.position = Vector3.Lerp(initialPosition, targetPosition.position, t);
            transformToLerp.rotation = Quaternion.Lerp(initialRotation, targetPosition.rotation, t);
            transformToLerp.localScale = Vector3.Lerp(initialScale, targetPosition.localScale, t);

            elapsedTime += Time.deltaTime;
            yield return  new WaitForFixedUpdate(); 
        }

    
        transformToLerp.position = targetPosition.position;
        transformToLerp.rotation = targetPosition.rotation;
    }

    public void SetTarget(Transform targetValue)
    {
        targetPosition = targetValue;
    }
        
    
    
}

