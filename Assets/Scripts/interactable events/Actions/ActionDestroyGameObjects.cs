
using UnityEngine;

public class ActionDestroyGameObjects : MonoBehaviour, IAction
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private bool destroyOnlyChildren =false;
    
    public  void StartAction(){
         foreach (GameObject obj in targets)
        {
            if (obj == null)
                return;
            if(!destroyOnlyChildren)
            {
                Destroy(obj);
            }
            else
            {
                 foreach (Transform child in obj.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    } 
}

