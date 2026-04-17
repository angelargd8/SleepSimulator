using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallingAsleepController : MonoBehaviour
{
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private DreamManager dreamManager;

    private void Start()
    {
        StartCoroutine(GoToRandomDream());
    }

    private IEnumerator GoToRandomDream()
    {
        yield return new WaitForSeconds(waitTime);

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
    }
}