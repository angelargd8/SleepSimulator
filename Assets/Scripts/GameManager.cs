
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] AudioClip ambienceClip;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayAmbience(ambienceClip);

    }
}
