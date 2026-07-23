
using UnityEngine;

public class ActionPlaySound : MonoBehaviour, IAction
{
    [SerializeField] private AudioSource sound;

    public  void StartAction()
    {
        sound.Play();
       
    }
}