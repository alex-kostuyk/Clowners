
using UnityEngine;

public class ActionShakeCamera : MonoBehaviour, IAction
{
    public float duration = 0.01f, intensity = 0.01f;
    public void StartAction()
    {
       // Camera.main.GetComponent<CameraShake>()?.StartShake(duration,intensity);
    }
}
