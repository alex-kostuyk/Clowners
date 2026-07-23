
using System.Collections;
using UnityEngine;

public class ActionFP_ControllerActivity : MonoBehaviour, IAction
{
    [SerializeField] private bool controller, rotation, limitYRotation,smoothYLimit, lockX = false;

    [SerializeField]
    [Range(-30f, -90f)]
    private float rotationLimit;
   /* private FP_Controller fp_controller;
    private PlayerPush playerPush;
    FP_CameraLook cameraLook;*/
    public  void StartAction()
    {
       /* if (playerPush == null)
        {
            init();
            if (playerPush == null)
                return;
        }

        fp_controller.enabled = controller;
        playerPush.notPushable = !controller;
        cameraLook.enabled = rotation;
        cameraLook.OnlyY = lockX;

        if(limitYRotation)
        {
            if (smoothYLimit)
                StartCoroutine(smoothLimit());
            else
                cameraLook.minimumY = rotationLimit;
        }
        else
            cameraLook.ResetYLimitToDefault();*/
    }

    private void init()
    {
     /*   fp_controller = FindObjectOfType<FP_Controller>();
        playerPush = fp_controller?.GetComponent<PlayerPush>();
        cameraLook = fp_controller?.GetComponent<FP_CameraLook>();*/
    }
    /*
    private IEnumerator smoothLimit()
    {
        float progress = 0;
        float progressSpeed = 0.09f;
        float startAngle = cameraLook.minimumY;

        while (progress < 1)
        {
            yield return new WaitForFixedUpdate();

            progress += progressSpeed;

            
              cameraLook.minimumY = Mathf.Lerp(startAngle, rotationLimit, progress);
        }

        cameraLook.minimumY = rotationLimit;
    }*/
}
