
using KinematicCharacterController.Examples;
using UnityEngine;

public class TpPlayerToPosition : MonoBehaviour, IAction
{
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateToTarget;
 
    private ExampleCharacterController playerCharController;
    public  void StartAction()
    {
        if (playerCharController == null)
        {
            playerCharController = getPlayerCharController();
            if(playerCharController == null)
                return;
            
        }
           

        playerCharController.Motor.ForceUnground();
        playerCharController.Motor.SetTransientPosition(target.position);
       


        if(rotateToTarget)
            playerCharController.Motor.SetRotation(target.rotation,false);
        
    }

    private ExampleCharacterController getPlayerCharController()
    {

        ExampleCharacterController [] players = FindObjectsOfType<ExampleCharacterController>();

        foreach (ExampleCharacterController player in players)
        {
            if (player.enabled)
                return player;
        }

        return null;

    }

}
      