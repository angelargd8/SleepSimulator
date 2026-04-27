using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Fan : MonoBehaviour, IInteractable
{
    public bool isOn = false;

    [SerializeField] private AudioClip fanSfx;
    

    public string GetInteractionText()
    {
        return isOn ? "Presiona E para apagar el ventilador" : "Presiona E para encender el ventilador";


    }


    public void Interact()
    {
        isOn = !isOn;

        UpdateFanState();

        Debug.Log("Ventilador: " + (isOn ? "Encendido" : "Apagado"));

    }

    void Start()
    {
        if (fanSfx == null)
        {
            Debug.Log("No se ha asignado ningun sonido");
        }


        UpdateFanState();
    }

    private void UpdateFanState()
    {

        if (AudioManager.Instance == null || fanSfx == null)
        {
            return;
        }

        if (isOn)
        {
            AudioManager.Instance.PlayLoopSFX(fanSfx);
        }
        else
        {
            AudioManager.Instance.StopLoopSFX(fanSfx);
        }
    }



}
