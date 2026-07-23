
using UnityEngine;

public class ActionPlayAnimation : MonoBehaviour, IAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;


    public  void StartAction()
    {
       if (animator != null && !string.IsNullOrEmpty(animationName))
        {
            animator.Play(animationName);
        }
    }
}
