using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable
{
    public bool isOn = true;
    //[SerializeField] lampLight;

    public string GetInteractionText()
    {
        return isOn ? "Presiona E para apagar la luz" : "Presiona E para encender la luz"; 
    }

    public void Interact()
    {
        isOn = !isOn;
        Debug.Log("Luz: " + (isOn ? "Encendida" : "Apagada"));

        //luego evento de onlightchanged
    }
}
