using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float delayBetweenPlays = 0.5f;
    private float lastPlayTime;

    private void OnEnable()
    {
        lastPlayTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (Time.time - lastPlayTime >= delayBetweenPlays)
        {
            audioSource.Play();
            
            lastPlayTime = Time.time;
        }
    }

    public void playSound() => audioSource.Play();


}
