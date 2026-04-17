using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedDreamController : MonoBehaviour
{
    [SerializeField] private float dreamDuration = 20f;
    [SerializeField] private string returnSceneName = "Bedroom";

    private void Start()
    {
        StartCoroutine(ReturnAfterTime());
    }

    private IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(dreamDuration);
        SceneManager.LoadScene(returnSceneName);
    }
}