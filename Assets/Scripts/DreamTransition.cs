using System.Collections;
using UnityEngine;

public class DreamTransition : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 5f;
    [SerializeField] private DreamManager dreamManager;

    private void Start()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(transitionDuration);

        if (dreamManager != null)
        {
            string dreamScene = dreamManager.GetRandomDreamScene();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(dreamScene);
            GameManager.instance.LoadScene(dreamScene);
        }
    }
}