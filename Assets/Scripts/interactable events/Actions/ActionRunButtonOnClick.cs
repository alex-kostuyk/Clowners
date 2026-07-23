using UnityEngine;
using UnityEngine.UI;

public class ActionRunButtonOnClick : MonoBehaviour, IAction
{
    [SerializeField] private Button button;

    public void StartAction(){
        button.onClick.Invoke(); 
    }
}
