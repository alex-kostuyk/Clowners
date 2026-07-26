
using UnityEngine;

public class ShowKeyMapHints : MonoBehaviour
{
    [SerializeField]
    private GameObject _hints;

    private bool hideStatus = false;
 

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            hideStatus = !hideStatus;

            _hints.SetActive(hideStatus);
        }
    }
}
