using UnityEngine;

public class DestroyOnSecondLoad : MonoBehaviour
{
    [SerializeField] private string flagKey = "HasBeenLoadedBefore";

    private void Awake()
    {
        if (PlayerPrefs.GetInt(flagKey, 0) == 1)
        {
            Destroy(gameObject);
        }
        else
        {
            PlayerPrefs.SetInt(flagKey, 1);
            PlayerPrefs.Save();
        }
    }
}