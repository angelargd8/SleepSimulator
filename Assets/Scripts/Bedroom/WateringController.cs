using UnityEngine;

public class WateringCupController : MonoBehaviour
{
    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioClip waterSFX;

    [Header("Detección de plantas")]
    [SerializeField] private Transform waterPoint;
    [SerializeField] private float waterRadius = 0.5f;

    private KeyCode waterKey = KeyCode.F;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (waterPoint == null)
        {
            waterPoint = transform;
        }
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
        if (animator != null)
        {
            animator.SetTrigger("Water");
        }

        if (AudioManager.Instance != null && waterSFX != null)
        {
            AudioManager.Instance.PlaySFX(waterSFX);
        }

        Collider[] hits = Physics.OverlapSphere(waterPoint.position, waterRadius);

        foreach (Collider hit in hits)
        {
            Plant plant = hit.GetComponentInParent<Plant>();

            if (plant != null)
            {
                plant.WaterPlant();
                return;
            }
        }

        Debug.Log("No hay una planta cerca para regar");
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = waterPoint != null ? waterPoint : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(point.position, waterRadius);
    }
}