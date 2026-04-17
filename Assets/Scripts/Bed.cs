using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform sleepPoint;
    [SerializeField] private SleepSystem sleepSystem;


    public string GetInteractionText()
    {
        return "Presiona E para dormir";
    }

    public void Interact()
    {
        Debug.Log("Durmiendo");
        

        if (sleepSystem != null && sleepPoint != null)
        {
            sleepSystem.StartSleeping(sleepPoint);
        }
    }
}
