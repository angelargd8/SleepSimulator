using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaLoader : MonoBehaviour
{
    [SerializeField] List<string> areasToLoad;
    [SerializeField] List<string> areasToUnLoad;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (string area in areasToLoad)
            {
                StartCoroutine(LoadArea(area));
            }
            foreach (string area in areasToUnLoad)
            {
                StartCoroutine (UnLoadArea(area));
            }
        }
    }

    IEnumerator LoadArea(string area)
    {
        if (!SceneManager.GetSceneByName(area).isLoaded)
        {
            SceneManager.LoadSceneAsync(area, LoadSceneMode.Additive);
        }

        yield return null;
    }

    IEnumerator UnLoadArea(string area)
    {
        if (SceneManager.GetSceneByName(area).isLoaded)
        {
            SceneManager.UnloadSceneAsync(area);
        }
        yield return null;

    }

}
