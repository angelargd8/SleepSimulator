using UnityEngine;

public class Boostrapper : MonoBehaviour
{
    public static Boostrapper instance;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(instance);
        }


    }
}
