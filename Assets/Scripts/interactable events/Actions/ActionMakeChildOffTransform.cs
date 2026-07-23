
using UnityEngine;

public class ActionMakeChildOffTransform : MonoBehaviour, IAction
{ 
    [SerializeField] private Transform child, parent;

    public void StartAction()
    {   
        child.parent = parent;
     
       // child.localPosition = Vector3.zero;
       // child.localRotation = Quaternion.identity;
    }

    public void SetParent(Transform newParent)
    {
        parent = newParent;
    }
        
    
    
}

