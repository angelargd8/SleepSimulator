using UnityEngine;

public class watercupPickUp : MonoBehaviour, IInteractable
{
    [Header("Punto donde se va a sostener")]
    [SerializeField] private Transform objectAnchor;

    [Header("Modelo visual")]
    [SerializeField] private Transform cupModel;
    [SerializeField] private Vector3 modelLocalPosition = Vector3.zero;
    [SerializeField] private Vector3 modelLocalRotation = new Vector3(0f, 90f, 0f);

    [Header("Opciones para botar")]
    [SerializeField] private float dropForwardDistance = 0.8f;
    [SerializeField] private float dropUpOffset = 0.2f;

    [Header("Colliders")]
    [SerializeField] private Collider interactionCollider;
    [SerializeField] private Collider physicalCollider;

    [Header("Offset al sostener")]
    [SerializeField] private Vector3 holdPositionOffset = new Vector3(0.166f, 0, -0.002f);
    [SerializeField] private Vector3 holdRotationOffset = new Vector3(0f, 0f, 0f);

    private bool pickedUp = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();

        if (interactionCollider == null)
        {
            interactionCollider = GetComponentInChildren<BoxCollider>();
        }

        if (physicalCollider == null && rb != null)
        {
            physicalCollider = rb.GetComponent<Collider>();
        }

        if (cupModel == null && rb != null)
        {
            cupModel = rb.transform;
        }
    }

    public string GetInteractionText()
    {
        if (pickedUp)
        {
            return "Presiona E para soltar el watering cup";
        }

        return "Presiona E para tomar el watering cup";
    }

    public void Interact()
    {
        if (pickedUp)
        {
            DropCup();
        }
        else
        {
            PickUpCup();
        }
    }

    private void PickUpCup()
    {
        pickedUp = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (physicalCollider != null)
        {
            physicalCollider.enabled = false;
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }

        transform.SetParent(objectAnchor, false);
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);

        
        if (cupModel != null)
        {
            cupModel.localPosition = modelLocalPosition;
            cupModel.localRotation = Quaternion.Euler(modelLocalRotation);
        }

        PlayerInventory.hasWateringCup = true;
        PlayerInventory.heldWateringCup = gameObject;

        Debug.Log("Watering cup tomado");
    }

    private void DropCup()
    {
        pickedUp = false;

        transform.SetParent(null, true);

        transform.position = objectAnchor.position + objectAnchor.forward * dropForwardDistance + Vector3.up * dropUpOffset;
        transform.rotation = Quaternion.identity;

        if (cupModel != null)
        {
            cupModel.localPosition = modelLocalPosition;
            cupModel.localRotation = Quaternion.Euler(modelLocalRotation);
        }

        if (physicalCollider != null)
        {
            physicalCollider.enabled = true;
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        PlayerInventory.hasWateringCup = false;
        PlayerInventory.heldWateringCup = null;

        Debug.Log("Watering cup soltado");
    }
}