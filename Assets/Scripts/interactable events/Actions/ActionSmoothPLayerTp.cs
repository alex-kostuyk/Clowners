using System.Collections;
using System.Threading;
using UnityEngine;

public class ActionSmoothPLayerTp : MonoBehaviour, IAction
{
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateToTarget, rotateHead;
    [SerializeField] private float newLocalHeadRotation,lerpSpeed=5;
    [SerializeField] private Event onLerpDone;
  //  private FP_Controller controller;
  //  private FP_CameraLook cameraLook;

    public void StartAction()
    {
      /*  if (controller == null)
        {
          //  controller = FindObjectOfType<FP_Controller>();
          //  cameraLook = controller.GetComponent<FP_CameraLook>();
        }

        StartCoroutine(lerpToTarget(controller.gameObject.transform,target.position,target.rotation,lerpSpeed));*/
   
    }

   /* private IEnumerator lerpToTarget(Transform player, Vector3 targetPosition,Quaternion TargetRotation, float speed = 1)
    {
     /*
        float startHeadRotation = cameraLook.rotationY;
        float time = 0;
        Vector3 initialPosition = player.transform.position;
        Quaternion initialRotation = player.transform.rotation;

       // NetworkTransform networkTransform = player.GetComponent<NetworkTransform>();

      
        //player.root.position = targetPosition;
        //player.position = initialPosition;



        while (time<1)
        {

            time +=Time.deltaTime*speed;

            if (rotateHead)
                cameraLook.rotationY = Mathf.Lerp(startHeadRotation, newLocalHeadRotation, time);

            if (rotateToTarget)
                player.SetPositionAndRotation(
                    Vector3.Lerp(initialPosition, targetPosition, time),
                    Quaternion.Lerp(initialRotation, TargetRotation, time)
                    );
            else
                player.position = Vector3.Lerp(initialPosition, targetPosition, time);

            yield return null;
        }

        
        player.SetPositionAndRotation(targetPosition, TargetRotation);
        if (rotateHead)
            cameraLook.SetYRotation(newLocalHeadRotation);

        onLerpDone?.CallActions();
     
    }*/

}
