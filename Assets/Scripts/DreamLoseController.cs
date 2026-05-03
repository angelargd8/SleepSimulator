using UnityEngine;

public class LoseDreamController : MonoBehaviour
{
    private bool hasLost = false;

    public void PlayerLost()
    {
        if (hasLost) return;

        hasLost = true;

        GameScoreData.cameFromDream = true;

        GameManager.instance.LoadNextSceneAfterDreamLoss();
    }
}