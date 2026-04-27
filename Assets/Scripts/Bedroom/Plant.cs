using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private bool isWatered = false;

    public void WaterPlant()
    {

        if (isWatered) return;

        isWatered = true;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddWateredPlant();
        }

        Debug.Log(gameObject.name + " fue regada");
    }
}