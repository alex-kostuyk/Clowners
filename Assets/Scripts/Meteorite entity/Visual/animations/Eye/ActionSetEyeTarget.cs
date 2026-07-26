using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSetEyeTarget : MonoBehaviour,IAction
{
    [SerializeField]
    private Transform newTarget;
    [SerializeField]
    private float targetedTime = 2;
    private Coroutine targetCoroutine;


    private MeteoriteEye meteoriteEye;

    private void Awake()
    {
        meteoriteEye = FindObjectOfType<MeteoriteEye>(true);
    }

    public void StartAction()
    {
        if (meteoriteEye == null)
            return;

        if (targetCoroutine != null)
        {
            StopCoroutine(targetCoroutine);
        }

        targetCoroutine = StartCoroutine(SetTargetTemporarilyRoutine());
    }

    private IEnumerator SetTargetTemporarilyRoutine()
    {
        
        meteoriteEye.Target = newTarget;

        yield return new WaitForSeconds(targetedTime);

        meteoriteEye.Target = null;

        targetCoroutine = null;
    }


}
