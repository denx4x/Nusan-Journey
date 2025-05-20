using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinInteract : MonoBehaviour
{
    public int sceneIndex;

    // Start is called before the first frame update
    public void Restart()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Update is called once per frame
    public void Quit()
    {
        Application.Quit();
    }
}
