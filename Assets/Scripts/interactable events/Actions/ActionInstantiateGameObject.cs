
using UnityEngine;

public class ActionInstantiateGameObject : MonoBehaviour, IAction
{
    [SerializeField] private GameObject instantiateObject;
    [SerializeField] private Transform spawn;
    public void StartAction()
    {
        Instantiate(instantiateObject,spawn.position,spawn.rotation);
    }
}
   