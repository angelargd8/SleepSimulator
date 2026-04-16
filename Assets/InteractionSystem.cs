using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    float distance = 3f;
    public LayerMask interactLayer;
    public TextMeshProUGUI interactionText;
    private IInteractable currentInteractable;

    void Update()
    {
        DetectInteraction();

        //if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        //{
        //    currentInteractable = null;
        //}
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
        }


    }

    void DetectInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, interactLayer))
        {

            //IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            //buscar el padre si ve al hijo
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.text = interactable.GetInteractionText();
                return;
            }
        }

        currentInteractable = null;
        interactionText.text = "";
    }

}
