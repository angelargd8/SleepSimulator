using System.Collections.Generic;
using UnityEngine;

public class DreamManager : MonoBehaviour
{
    [SerializeField] private List<string> dreamScenes = new List<string>();

    public string GetRandomDreamScene()
    {
        if (dreamScenes == null || dreamScenes.Count == 0)
        {
            Debug.LogError("No hay escenas de sueño asignadas.");
            return "";
        }

        int index = Random.Range(0, dreamScenes.Count);
        return dreamScenes[index];
    }
}