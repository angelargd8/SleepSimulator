using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour, IInteractable
{
    [Header("Estado de la TV")]
    [SerializeField] private bool isOn = false;

    [Header("Pantalla con video")]
    [SerializeField] private GameObject videoScreenObject;
    [SerializeField] private VideoPlayer tvVideo;

    [Header("Pantalla apagada")]
    [SerializeField] private GameObject screenOffObject;

    public string GetInteractionText()
    {
        return isOn ? "Presiona E para apagar la TV" : "Presiona E para encender la TV";
    }

    public void Interact()
    {
        isOn = !isOn;
        UpdateTVState();

        Debug.Log("TV: " + (isOn ? "Encendida" : "Apagada"));
    }

    private void Start()
    {
        if (tvVideo == null)
        {
            tvVideo = GetComponentInChildren<VideoPlayer>(true);
        }

        if (tvVideo != null && videoScreenObject == null)
        {
            videoScreenObject = tvVideo.gameObject;
        }

        UpdateTVState();
    }

    private void UpdateTVState()
    {
        if (videoScreenObject != null)
        {
            videoScreenObject.SetActive(isOn);
        }

        if (screenOffObject != null)
        {
            screenOffObject.SetActive(!isOn);
        }

        if (tvVideo == null)
        {
            Debug.LogWarning("No se hay ningun VideoPlayer a la TV.");
            return;
        }

        if (isOn)
        {
            tvVideo.Play();
            tvVideo.SetDirectAudioMute(0, false);
        }
        else
        {
            tvVideo.Pause();
            tvVideo.SetDirectAudioMute(0, true);
        }
    }
}