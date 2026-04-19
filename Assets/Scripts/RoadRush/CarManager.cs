using TMPro;
using UnityEngine;

public class CarManager : MonoBehaviour
{

    public static CarManager instance;

    [Header("Wheels")]
    [SerializeField] private TMP_Text wheelsText;

    private int wheels = 0;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddWheel(int amount)
    {

        wheels += amount;
        UpdateUI();

    }

    private void UpdateUI()
    {
        wheelsText.text = "Wheels: " + wheels;
    }


}
