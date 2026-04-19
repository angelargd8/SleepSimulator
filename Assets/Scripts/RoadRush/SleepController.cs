using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SleepController : MonoBehaviour
{
    [SerializeField] private float waitTime = 5f;
    [SerializeField] private DreamManager dreamManager;
    [SerializeField] private TMP_Text scoreText;

    [Header("Escena de regreso")]
    [SerializeField] private string returnSceneName = "Bedroom";

    [Header("Probabilidades")]
    [SerializeField] private float bedroomChance = 0.50f;

    private void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score:\n" + GameScoreData.finalScore;
        }

        if (GameScoreData.cameFromDream)
        {
            GameScoreData.cameFromDream = false;
            return;
        }

        StartCoroutine(GoToRandomScene());
    }

    private IEnumerator GoToRandomScene()
    {
        yield return new WaitForSeconds(waitTime);

        float randomValue = Random.value;

        if (randomValue < bedroomChance)
        {
            SceneManager.LoadScene(returnSceneName);
            yield break;
        }

        if (dreamManager == null)
        {
            Debug.LogError("DreamManager no está asignado.");
            yield break;
        }

        string selectedScene = dreamManager.GetRandomDreamScene();

        if (!string.IsNullOrEmpty(selectedScene))
        {
            SceneManager.LoadScene(selectedScene);
        }
        else
        {
            Debug.LogWarning("Regresando a Bedroom.");
            SceneManager.LoadScene(returnSceneName);
        }
    }
}