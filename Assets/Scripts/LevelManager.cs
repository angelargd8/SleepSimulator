using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Contador de plantas")]
    [SerializeField] private int wateredPlantsCount = 0;
    //private int wateredPlantsGoal = 18;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddWateredPlant()
    {
        wateredPlantsCount++;

        Debug.Log("Plantas regadas: " + wateredPlantsCount);
    }

}