using UnityEngine;

public class MainMenu : MonoBehaviour
{
  

    public void LoadLevel(string startLevel)
    {
        GameManager.instance.LoadScene(startLevel);
    }

}
