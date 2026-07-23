
using UnityEngine;

public class ActionSetGameobjectActivity : MonoBehaviour, IAction
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private bool state,flipSelfActivity,changeOneRandom;

    public void StartAction(){
        if(changeOneRandom)
        {
           setActivity(targets[Random.Range(0,targets.Length)]);
            return;
        }

         foreach (GameObject obj in targets)
        {
           setActivity(obj);
        }
    }
    private void setActivity(GameObject obj)
    {
        if (obj != null)
            {
                if(flipSelfActivity)
                    state = !obj.activeSelf;
                obj.SetActive(state);
            }
    }
}
