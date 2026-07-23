
using UnityEngine;

public class ActionPlayRandomeSound : MonoBehaviour, IAction
{
 
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource sound;
    [SerializeField] private bool noRepeatLast = true,randomizePitch = true;
    [SerializeField]
    [Range(0, 50)]
    private int pitchVariation;
    private int lastIndex,index;

    public  void StartAction()
    {
        index = audioClips.Length==1?0:Random.Range(0,audioClips.Length);
        if (noRepeatLast && lastIndex == index)
        {
            StartAction();
            return;
        }

        sound.clip = audioClips[index];
        lastIndex = index;

        if(randomizePitch ) 
            sound.pitch = 1 * Random.Range(100f-pitchVariation,100f+pitchVariation)/100;

        sound.Play();


       


    }
}