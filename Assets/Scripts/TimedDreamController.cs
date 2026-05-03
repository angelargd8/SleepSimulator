using System.Collections;
using UnityEngine;


public class TimedDreamController : MonoBehaviour
{

    [SerializeField] private float dreamDuration = 60f;


    private bool hasFinished = false;
    private Coroutine timerCoroutine;

    private void OnEnable()
    {
        timerCoroutine = StartCoroutine(ReturnAfterTime());
    }

    private void OnDisable()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    private IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(dreamDuration);

        FinishDream();
    }

    public void FinishDream()
    {
        if (hasFinished) return;

        hasFinished = true;

        if (GameManager.instance != null)
        {
            GameScoreData.cameFromDream = true;
            GameManager.instance.LoadNextSceneAfterDreamLoss();
        }
        else
        {
            Debug.LogError("No existe GameManager en la escena.");
        }
    }
}