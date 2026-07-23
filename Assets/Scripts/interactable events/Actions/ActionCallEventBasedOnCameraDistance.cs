using UnityEngine;

public class ActionCallEventBasedOnCameraDistance : MonoBehaviour, IAction
{
    [SerializeField]
    private float distance = 10;
    [SerializeField]
    private Event targetEvent,failedEvent;
    public void StartAction()
    {
        Transform mainCamera = Camera.main.transform;

        if (mainCamera == null)
            return;

        if (Vector3.Distance(transform.position, mainCamera.transform.position) <= distance)
            targetEvent?.CallActions();
        else
            failedEvent?.CallActions();

    }
}
