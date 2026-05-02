
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
            return;
        }

        
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }


    IEnumerator LoadSceneAsync(string sceneName)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLoadingMusic();
        }

        //TODO: aqui luego editar otras ciertas escenas y manejar mejor el redirijir a otras escenas

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

        yield return null;

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);


        operation.allowSceneActivation = true;


        while (!operation.isDone)
        {
            yield return null;
        }

        if (SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync("LoadingScene");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicForScene(sceneName);
        }
    }




}
