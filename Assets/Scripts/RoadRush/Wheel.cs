using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] AudioClip WheelSFX;
    public float rotationSpeed = 180f;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }


        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(WheelSFX);
            CarManager.instance.AddWheel(1);
            gameObject.SetActive(false);
        }
        else
        {
            return;
        }

    }

    public void ResetWheel()
    {
        gameObject.SetActive(true);
    }

    public void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    


}
