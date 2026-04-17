using System.Collections;
using UnityEngine;

public class SleepSystem : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private float sleepDelay = 3f;

    private bool isSleeping = false;

    public void StartSleeping(Transform sleepPoint)
    {
        if (isSleeping) return;

        StartCoroutine(SleepRoutine(sleepPoint));
    }

    private IEnumerator SleepRoutine(Transform sleepPoint)
    {
        isSleeping = true;

        // desactivar movimiento
        if (playerController != null)
            playerController.enabled = false;

        // mover al jugador a la cama
        player.transform.position = sleepPoint.position;

        // rotación acostado
        player.transform.rotation = sleepPoint.rotation;

        // mover el jugador 90 grados en X:
        player.transform.rotation = Quaternion.Euler(270f, sleepPoint.eulerAngles.y, sleepPoint.eulerAngles.z);

        yield return new WaitForSeconds(sleepDelay);

        if (sceneLoader != null)
        {
            sceneLoader.LoadScene("FallingAsleep");
        }
    }
}