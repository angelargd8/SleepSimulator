using UnityEngine;

public class WateringCupController : MonoBehaviour
{
    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioClip waterSFX;

    private KeyCode waterKey = KeyCode.F;
    
    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!PlayerInventory.hasWateringCup) return;

        if (Input.GetKeyDown(waterKey))
        {
            WaterPlants();
        }
    }

    private void WaterPlants()
    {
        if (animator == null)
        {
            Debug.LogWarning("No se ha asignado el Animator del WateringCup.");
            return;
        }

        AudioManager.Instance.PlaySFX(waterSFX);
        animator.SetTrigger("Water");
    }
}