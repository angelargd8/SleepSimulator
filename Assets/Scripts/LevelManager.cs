using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Contador de plantas")]
    [SerializeField] private int wateredPlantsCount = 0;
    [SerializeField] private TMP_Text wateredPlantsText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateWateredPlantsUI();
    }

    public void AddWateredPlant()
    {
        wateredPlantsCount++;
        UpdateWateredPlantsUI();

        Debug.Log("Plantas regadas: " + wateredPlantsCount);
    }

    private void UpdateWateredPlantsUI()
    {
        if (wateredPlantsText != null)
        {
            wateredPlantsText.text = "Plantas regadas: " + wateredPlantsCount;
        }
    }
}