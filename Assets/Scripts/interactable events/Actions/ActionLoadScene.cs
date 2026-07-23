using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionLoadScene : MonoBehaviour, IAction
{

    [SerializeField] private int sceneIndexToLoad;

    public void StartAction()
    {
       SceneManager.LoadScene(sceneIndexToLoad);
    }
}
