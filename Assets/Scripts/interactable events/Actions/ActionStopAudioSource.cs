
using UnityEngine;

public class ActionStopAudioSource : MonoBehaviour, IAction
{
    [SerializeField] private AudioSource sound;

    public void StartAction()
    {
        if(sound != null)
            sound.Stop();
    }
}