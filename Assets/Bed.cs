using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public string GetInteractionText()
    {
        return "Presiona E para dormir";
    }

    public void Interact()
    {
        Debug.Log("Durmiendo");
        //luego disparar evento de onplayersleep attempt
    }
}
