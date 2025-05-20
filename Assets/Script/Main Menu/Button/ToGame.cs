using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToGame : MonoBehaviour {    
    [SerializeField] private int index;

    public void PlayGame() {
        SceneManager.LoadScene(index);
        Time.timeScale = 1f;
    }

}