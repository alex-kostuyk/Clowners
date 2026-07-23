
using UnityEngine;

public class ActionSetAnimatorState : MonoBehaviour, IAction
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private bool state;
    public void StartAction()
    {
        animator.enabled = state;
    }
}
