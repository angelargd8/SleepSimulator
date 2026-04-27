using UnityEngine;

public class Boostrapper : MonoBehaviour
{
    public static Boostrapper instance;

    private void Awake()
    {
        if(instance == null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
