using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    private bool isPaused;  
    public int mainMenu;

    public GameObject pausePanel;            

    public GameObject settingsPanel;

    public void GamePause() {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0;        
    }

    public void ResumeGame() {
        pausePanel.SetActive(false);        
        Time.timeScale = 1;
    }

    public void SettingGame() {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void QuitGame() {
        SceneManager.LoadScene(mainMenu);
        Time.timeScale = 1;
    }

    

}






















