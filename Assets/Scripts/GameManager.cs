
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Escenas")]
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private string bedroomSceneName = "Bedroom";

    [Header("Sueños")]
    [SerializeField] private List<string> dreamScenes = new List<string>();

    [Header("Probabilidades")]
    [SerializeField] private float bedroomChance = 0.50f;

    [Header("Loading")]
    [SerializeField] private float loadingMinTime = 3.0f;

    private bool isLoading = false;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, false);
    }

    public void LoadScene(string sceneName, bool showScore)
    {
        if (isLoading) return;

        StartCoroutine(LoadSceneAsync(sceneName, showScore));
    }

    public void LoadNextSceneAfterDreamLoss()
    {
        string nextScene = GetNextSceneAfterDream();

        LoadingScreenData.showScore = true;
        LoadingScreenData.score = GameScoreData.finalScore;
        LoadingScreenData.targetScene = nextScene;

        LoadScene(nextScene, true);
    }

    private string GetNextSceneAfterDream()
    {
        float randomValue = Random.value;

        if (randomValue < bedroomChance)
        {
            return bedroomSceneName;
        }

        if (dreamScenes == null || dreamScenes.Count == 0)
        {
            Debug.LogWarning("No hay escenas de sueño asignadas. Regresando a Bedroom.");
            return bedroomSceneName;
        }

        int index = Random.Range(0, dreamScenes.Count);
        return dreamScenes[index];
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool showScore)
    {
        isLoading = true;

        LoadingScreenData.showScore = showScore;
        LoadingScreenData.score = GameScoreData.finalScore;
        LoadingScreenData.targetScene = sceneName;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLoadingMusic();
        }

        if (!SceneManager.GetSceneByName(loadingSceneName).isLoaded)
        {
            SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Additive);

        }

        
        yield return null;

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(loadingMinTime);


        operation.allowSceneActivation = true;


        while (!operation.isDone)
        {
            yield return null;
        }

        if (SceneManager.GetSceneByName(loadingSceneName).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicForScene(sceneName);
        }

        LoadingScreenData.showScore = false;
        LoadingScreenData.targetScene = "";

        isLoading = false;
    }




}
