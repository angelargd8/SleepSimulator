using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform sleepPoint;
    [SerializeField] private SleepSystem sleepSystem;

    [SerializeField] private Lamp lamp;

    public string GetInteractionText()
    {
        if (lamp != null && lamp.isOn)
            return "Apaga la luz para dormir";


        return "Presiona E para dormir";
    }

    public void Interact()
    {
        if (lamp != null && lamp.isOn)
        {
            Debug.Log("No puedes dormir con la luz encendida.");
            return;
        }

        if (sleepSystem != null && sleepPoint != null)
        {
            sleepSystem.StartSleeping(sleepPoint);
   
        }
    }
}
