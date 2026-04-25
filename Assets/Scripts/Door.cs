using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Estado de la puerta")]
    [SerializeField] private bool isOpen = false;

    [Header("puerta cerrada")]
    [SerializeField] private GameObject closedDoorObject;

    [Header("puerta abierta")]
    [SerializeField] private GameObject openDoorObject;

    public string GetInteractionText()
    {
        return isOpen ? "Presiona E para cerrar la puerta" : "Presiona E para abrir la puerta";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        UpdateDoorState();

        Debug.Log("Puerta: " + (isOpen ? "Abierta" : "Cerrada"));
    }

    private void Start()
    {
        if (closedDoorObject == null)
        {
            closedDoorObject = gameObject;
        }

        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        if (closedDoorObject != null)
        {
            closedDoorObject.SetActive(!isOpen);
        }

        if (openDoorObject != null)
        {
            openDoorObject.SetActive(isOpen);
        }
    }
}