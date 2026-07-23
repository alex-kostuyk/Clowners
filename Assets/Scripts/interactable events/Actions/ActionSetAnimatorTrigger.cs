
using UnityEngine;

public class ActionSetAnimatorTrigger : MonoBehaviour, IAction
{
     [SerializeField] private Animator animator;
     [SerializeField] private string triggerName;
      public  void StartAction()
      {
            animator.SetTrigger(triggerName);
      }

}
