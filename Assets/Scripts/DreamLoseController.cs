using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseDreamController : MonoBehaviour
{
    [SerializeField] private string returnSceneName = "Sleeping";
    //[SerializeField] private string returnSceneName = "Bedroom";


    private bool hasLost = false;

    public void PlayerLost()
    {
        if (hasLost) return;

        hasLost = true;
        SceneManager.LoadScene(returnSceneName);
    }
}