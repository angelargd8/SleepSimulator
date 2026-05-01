
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

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


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }


    IEnumerator LoadSceneAsync(string sceneName)
    {
                
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;


        //TODO: aqui luego editar otras ciertas escenas y manejar mejor el redirijir a otras escenas

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        while (operation.progress < 0.9)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);

        SceneManager.UnloadSceneAsync("LoadingScene");
        operation.allowSceneActivation = true;


        while (!operation.isDone)
        {
            yield return null;
        }


    }




}
