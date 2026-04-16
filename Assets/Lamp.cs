using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class Lamp : MonoBehaviour, IInteractable
{
    public bool isOn = true;
    [SerializeField] private Light lampLight;

    public float onRange = 2.5f;
    public float offRange = 0f;

    public string GetInteractionText()
    {
        return isOn ? "Presiona E para apagar la luz" : "Presiona E para encender la luz"; 
    }

    public void Interact()
    {
        isOn = !isOn;

        if (lampLight  != null )
        {
            StartCoroutine(FadeLight(isOn ? onRange : offRange));
        }

        Debug.Log("Luz: " + (isOn ? "Encendida" : "Apagada"));

        //luego evento de onlightchanged
    }

    public void Start()
    {
        if (lampLight != null)
        {
            lampLight.range = isOn ? onRange : offRange;
        }
    }

    IEnumerator FadeLight(float targetRange)
    {
        float start = lampLight.range;
        float time = 0f;

        while (time < 0.3f)
        {
            lampLight.range = Mathf.Lerp(start, targetRange, time / 0.3f);
            time += Time.deltaTime;
            yield return null;
        }

        lampLight.range = targetRange;
    }


}
