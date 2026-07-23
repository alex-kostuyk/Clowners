
using UnityEngine;

public class ActionPlayParticleSystem : MonoBehaviour, IAction
{
    [SerializeField] private new ParticleSystem particleSystem;

    public  void StartAction(){
        particleSystem.Play();
    }
}