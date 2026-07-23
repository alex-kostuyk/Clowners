
using UnityEngine;

public class ActionSetMonobiheaviorActivity : MonoBehaviour, IAction
{
    [SerializeField] private MonoBehaviour script;
    [SerializeField] private bool state,flipCurrentState,delete;

    public void StartAction(){

        if (flipCurrentState) 
        {
            script.enabled = !script.enabled;
            return;
        }
        script.enabled = state;

        if(delete)
            Destroy(script);
    }
}