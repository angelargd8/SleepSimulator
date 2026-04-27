using UnityEngine;
using UnityEngine.SceneManagement;


public class BoostrapLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    static void Init()
    {
        if (Object.FindFirstObjectByType<Boostrapper>() != null)
            return;

        SceneManager.LoadScene("Boostrap",LoadSceneMode.Additive);
    }
}
