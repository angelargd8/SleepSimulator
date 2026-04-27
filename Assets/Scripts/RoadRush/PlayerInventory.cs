using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static bool hasWateringCup = false;

    public static GameObject heldWateringCup = null;

    // consumir cup
    public static void ConsumeCup()
    {
        hasWateringCup = false;

        if (heldWateringCup != null )
        
            {
                Destroy(heldWateringCup);
                heldWateringCup = null;
            }
    }



}
