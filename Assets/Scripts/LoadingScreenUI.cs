using TMPro;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        if (scoreText == null) return;

        if (LoadingScreenData.showScore)
        {
            scoreText.text = "Score: " + LoadingScreenData.score;
        }
        else
        {
            scoreText.text = "";
        }
    }
}